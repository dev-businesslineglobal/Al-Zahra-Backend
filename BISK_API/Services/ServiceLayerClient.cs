using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using gardnerAPIs.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using GardeningAPI.Application.Interfaces;
using GardeningAPI.Model;
using Newtonsoft.Json.Serialization;

namespace BISK_API.Services
{
    /// Auto-managed SAP B1 Service Layer v2 client:
    ///  - Logs in on demand with technical user from config
    ///  - Keeps session via cookies + local expiry timestamp
    ///  - Retries once on 401 / "Invalid session"/"Session timeout"
    ///  - Can BYPASS SSL if ServiceLayerOptions.SkipTlsVerify=true (DEV ONLY)
    public sealed class ServiceLayerClient : IServiceLayerClient, IDisposable
    {
        private readonly HttpClient _http;               // active client
        private readonly HttpClient? _ownedUnsafeHttp;   // when we build unsafe handler
        private readonly ServiceLayerOptions _opt;
        private readonly SemaphoreSlim _authGate = new(1, 1);

        private DateTimeOffset _sessionExpiresUtc = DateTimeOffset.MinValue;

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public ServiceLayerClient(HttpClient httpFromDi, IOptions<ServiceLayerOptions> opt)
        {
            _opt = opt?.Value ?? throw new ArgumentNullException(nameof(opt));

            if (_opt.SkipTlsVerify)
            {
                var handler = new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = new CookieContainer(),
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                _ownedUnsafeHttp = new HttpClient(handler)
                {
                    BaseAddress = new Uri(_opt.Url.TrimEnd('/') + "/"),
                    Timeout = TimeSpan.FromSeconds(60)
                };
                _http = _ownedUnsafeHttp;
            }
            else
            {
                _http = httpFromDi ?? throw new ArgumentNullException(nameof(httpFromDi));
            }

            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!_http.DefaultRequestHeaders.Contains("Prefer"))
                _http.DefaultRequestHeaders.Add("Prefer", "return=representation");
        }

        public void Dispose()
        {
            _ownedUnsafeHttp?.Dispose();
            _authGate.Dispose();
        }

        private bool SessionValid() => DateTimeOffset.UtcNow < _sessionExpiresUtc;

        private async Task EnsureLoggedInAsync(CancellationToken ct)
        {
            if (SessionValid()) return;

            await _authGate.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (SessionValid()) return;

                var loginBody = new
                {
                    CompanyDB = _opt.CompanyDB,
                    UserName = _opt.UserName,
                    Password = _opt.Password
                };

                using var req = new HttpRequestMessage(HttpMethod.Post, "Login")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(loginBody), Encoding.UTF8, "application/json")
                };

                using var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
                if (!res.IsSuccessStatusCode)
                    throw await BuildException(res).ConfigureAwait(false);

                var text = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                var payload = string.IsNullOrWhiteSpace(text) ? null : JsonConvert.DeserializeObject<LoginResponse>(text);
                var minutes = Math.Max(1, payload?.SessionTimeout ?? _opt.SessionTtlMinutes);
                _sessionExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(minutes - 1);
            }
            finally
            {
                _authGate.Release();
            }
        }

        private static async Task<bool> IsSessionInvalidAsync(HttpResponseMessage res)
        {
            if (res.IsSuccessStatusCode) return false;
            try
            {
                var text = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                var err = JsonConvert.DeserializeObject<SlErrorEnvelope>(text ?? "");
                var msg = err?.error?.message?.value ?? "";
                return msg.Contains("Invalid session", StringComparison.OrdinalIgnoreCase)
                    || msg.Contains("Session timeout", StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }

        private static async Task<InvalidOperationException> BuildException(HttpResponseMessage res)
        {
            var msg = $"HTTP {(int)res.StatusCode} {res.ReasonPhrase}";
            try
            {
                var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                var err = JsonConvert.DeserializeObject<SlErrorEnvelope>(body ?? "");
                var txt = BestMessage(err?.error?.message?.value, body, res.ReasonPhrase);
                if (!string.IsNullOrWhiteSpace(txt)) msg += $": {txt}";
            }
            catch { /* ignore */ }
            return new InvalidOperationException(msg);
        }

        private static async Task<OperationResult<T>> ToResult<T>(HttpResponseMessage res)
        {
            var r = new OperationResult<T> { HttpStatus = (int)res.StatusCode };
            var content = res.Content == null ? null : await res.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (res.IsSuccessStatusCode)
            {
                r.IsSuccess = true;
                if (!string.IsNullOrWhiteSpace(content))
                    r.Result = JsonConvert.DeserializeObject<T>(content);
                return r;
            }

            try
            {
                var err = JsonConvert.DeserializeObject<SlErrorEnvelope>(content ?? "");
                if (err?.error != null)
                {
                    r.IsSuccess = false;
                    r.ErrorMessages.Add(new ErrorResponse
                    {
                        error = new ErrorDetails
                        {
                            code = SafeToInt(err.error.code),
                            message = new ErrorMessage
                            {
                                value = BestMessage(err.error.message?.value, content, res.ReasonPhrase)
                            }
                        },
                        httpStatus = (int)res.StatusCode
                    });
                    return r;
                }
            }
            catch { /* ignore */ }

            r.IsSuccess = false;
            r.ErrorMessages.Add(new ErrorResponse
            {
                error = new ErrorDetails
                {
                    code = (int)res.StatusCode,
                    message = new ErrorMessage { value = BestMessage(null, content, res.ReasonPhrase) }
                },
                httpStatus = (int)res.StatusCode
            });
            return r;
        }
        private static string BestMessage(string primary, string fallbackContent, string reason)
        {
            var msg = UnwrapInnerJsonMessage(primary);
            if (!string.IsNullOrWhiteSpace(msg)) return msg;

            msg = UnwrapInnerJsonMessage(fallbackContent);
            if (!string.IsNullOrWhiteSpace(msg)) return msg;

            if (!string.IsNullOrWhiteSpace(primary) && !LooksLikeJson(primary))
                return primary.Trim();

            return string.IsNullOrWhiteSpace(reason) ? "Error" : reason.Trim();
        }

        private static bool LooksLikeJson(string s)
            => !string.IsNullOrWhiteSpace(s) && (s.TrimStart().StartsWith("{") || s.TrimStart().StartsWith("["));

        private static string? UnwrapInnerJsonMessage(string maybeJson)
        {
            if (!LooksLikeJson(maybeJson)) return null;
            try
            {
                var env = JsonConvert.DeserializeObject<InnerSapEnvelope>(maybeJson);
                var m = env?.error?.message;
                if (!string.IsNullOrWhiteSpace(m)) return m.Trim();

                var d = env?.error?.details;
                if (d != null)
                {
                    foreach (var it in d)
                        if (!string.IsNullOrWhiteSpace(it?.message))
                            return it.message.Trim();
                }
            }
            catch { /* ignore */ }
            return null;
        }

        private static int SafeToInt(object code) { try { return Convert.ToInt32(code); } catch { return -1; } }

        // ============================ Public Business Partner Methods ============================

        public Task<OperationResult<Response>> AddSaleOrder(Document doc, CancellationToken ct = default) => Post<Response>("Orders", doc, ct);
        public Task<OperationResult<Response>> AddDelivery(Document doc, CancellationToken ct = default) => Post<Response>("DeliveryNotes", doc, ct);
        public Task<OperationResult<Response>> AddInvoices(Document doc, CancellationToken ct = default) => Post<Response>("Invoices", doc, ct);
        public Task<OperationResult<Response>> AddCreditNotes(Document doc, CancellationToken ct = default) => Post<Response>("CreditNotes", doc, ct);
        public Task<OperationResult<Response>> AddIncomingPayment(IncomingPayment payment, CancellationToken ct = default) => Post<Response>("IncomingPayments", payment, ct);

        public Task<OperationResult<ResponseBP>> AddBP(SignUp bp, CancellationToken ct = default) => Post<ResponseBP>("BusinessPartners", bp, ct);
        public Task<OperationResult<ResponseBP>> PatchBP(string cardCode, SignUp patch, CancellationToken ct = default)
            => Patch<ResponseBP>($"BusinessPartners('{Uri.EscapeDataString(cardCode)}')", patch, ct);


        public Task<OperationResult<ResponseBP>> PatchBPU(string cardCode, object patch, CancellationToken ct = default)
            => Patch<ResponseBP>($"BusinessPartners('{Uri.EscapeDataString(cardCode)}')", patch, ct);


        public Task<OperationResult<ResponseItem>> AddCart(Drafts draft, CancellationToken ct = default) => Post<ResponseItem>("Drafts", draft, ct);
        public Task<OperationResult<ResponseItem>> PutCart(string cardCode, Drafts put, CancellationToken ct = default)
            => Put<ResponseItem>($"Drafts('{Uri.EscapeDataString(cardCode)}')", put, ct);



        // ============================ OTP Methods ============================
        public Task<OperationResult<ResponseOTP>> AddOTP(OTP OtPbp, CancellationToken ct = default) => Post<ResponseOTP>("OTP_T", OtPbp, ct);
        public Task<OperationResult<OTP>> PatchOtp(string code, object patch, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("OTP Code is required.", nameof(code));

            return Patch<OTP>($"OTP_T('{Uri.EscapeDataString(code)}')", patch, ct);
        }

        // public Task<OperationResult<ResponseOTP>> PatchBPOTP(string Code, OTP patch, CancellationToken ct = default)
        //=> Patch<ResponseOTP>($"U_OTP_TABLE('{Uri.EscapeDataString(Code)}')", patch, ct);

        //public Task<OperationResult<ResponseItem>> AddItem(ItemMaster item, CancellationToken ct = default) => Post<ResponseItem>("Items", item, ct);
        //public Task<OperationResult<ResponseItem>> PatchItem(string itemCode, ItemMasterPatch patch, CancellationToken ct = default)
        //    => Patch<ResponseItem>($"Items('{Uri.EscapeDataString(itemCode)}')", patch, ct);


        public async Task<OperationResult<OTP>> GetOtpFromUDOAsync(string email, string otpCode, CancellationToken ct = default)
        {
            try
            {
                // Construct the filter query
                string filter = $"?$filter=U_Email eq '{email}' and U_OTPCode eq '{otpCode}'";
                using var res = await SendWithAuthAsync(HttpMethod.Get, $"U_OTP_TABLE{filter}", null, ct).ConfigureAwait(false);
                var result = await ToResult<OTP>(res).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    var errors = string.Join(", ", result.ErrorMessages.Select(e => e.error?.message?.value));
                    throw new InvalidOperationException($"Failed to retrieve OTP: {errors}");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Exception while retrieving OTP: {ex.Message}", ex);
            }
        }


        //public async Task<OperationResult<OTP>> PatchOtpToUDOAsync(OTP otp, CancellationToken ct = default)
        //{
        //    try
        //    {
        //        var body = new
        //        {
        //            U_OTPCode = otp.U_OTPCode,
        //            U_CreatedAt = otp.U_CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        //            U_ExpireAt = otp.U_ExpireAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        //            U_IsUsed = otp.U_IsUsed
        //        };
        //        // PATCH to your UDO table using the Code as identifier
        //        //var result = await Patch<OTP>($"U_OTP_TABLE('{Uri.EscapeDataString(otp.Email)}')", body, ct);
        //        var result = await Patch<OTP>($"OTP_T('{otp.Code}')", body, ct);
        //        if (!result.IsSuccess)
        //        {
        //            var errors = string.Join(", ", result.ErrorMessages.Select(e => e.error?.message?.value));
        //            throw new InvalidOperationException($"Failed to update OTP: {errors}");
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidOperationException($"Exception while updating OTP: {ex.Message}", ex);
        //    }
        //}


        // ============================ Helpers Methods ============================
        private async Task<OperationResult<T>> Post<T>(string EndPoint, object payload, CancellationToken ct)
        {
            using var res = await SendWithAuthAsync(HttpMethod.Post, EndPoint, payload, ct).ConfigureAwait(false);
            return await ToResult<T>(res).ConfigureAwait(false);
        }
        private async Task<OperationResult<T>> Patch<T>(string table, object payload, CancellationToken ct)
        {
            using var res = await SendWithAuthAsync(new HttpMethod("PATCH"), table, payload, ct).ConfigureAwait(false);
            return await ToResult<T>(res).ConfigureAwait(false);
        }

        private async Task<OperationResult<T>> Put<T>(string table, object payload, CancellationToken ct)
        {
            using var res = await SendWithAuthAsync(new HttpMethod("PUT"), table, payload, ct).ConfigureAwait(false);
            return await ToResult<T>(res).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> SendWithAuthAsync(HttpMethod method, string path, object? body, CancellationToken ct)
        {
            try
            {
                await EnsureLoggedInAsync(ct).ConfigureAwait(false);

                using var req = new HttpRequestMessage(method, path);
                if (body != null)
                    req.Content = new StringContent(JsonConvert.SerializeObject(body, _jsonSettings), Encoding.UTF8, "application/json");

                var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

                // Retry on Unauthorized or session invalid
                if (res.StatusCode == HttpStatusCode.Unauthorized || await IsSessionInvalidAsync(res).ConfigureAwait(false))
                {
                    res.Dispose();
                    _sessionExpiresUtc = DateTimeOffset.MinValue; // force re-login
                    await EnsureLoggedInAsync(ct).ConfigureAwait(false);

                    using var retry = new HttpRequestMessage(method, path)
                    {
                        Content = body == null ? null : new StringContent(JsonConvert.SerializeObject(body, _jsonSettings), Encoding.UTF8, "application/json")
                    };
                    res = await _http.SendAsync(retry, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
                }

                // Log the response content if not successful
                if (!res.IsSuccessStatusCode)
                {
                    var errorContent = await res.Content.ReadAsStringAsync();
                    Console.WriteLine($"Request to {path} failed with status {res.StatusCode}:\n{errorContent}");
                    // Optional: throw here if you want the caller to handle it
                    // throw new InvalidOperationException($"SAP request failed: {errorContent}");
                }

                return res;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception during SAP request to {path}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw; // rethrow so caller can handle it if needed
            }
        }




        //private async Task<HttpResponseMessage> SendWithAuthAsync(HttpMethod method, string path, object? body, CancellationToken ct)
        //{
        //    await EnsureLoggedInAsync(ct).ConfigureAwait(false);

        //    using var req = new HttpRequestMessage(method, path);
        //    if (body != null)
        //        req.Content = new StringContent(JsonConvert.SerializeObject(body, _jsonSettings), Encoding.UTF8, "application/json");

        //    var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

        //    if (res.StatusCode == HttpStatusCode.Unauthorized || await IsSessionInvalidAsync(res).ConfigureAwait(false))
        //    {
        //        res.Dispose();
        //        _sessionExpiresUtc = DateTimeOffset.MinValue; // force re-login
        //        await EnsureLoggedInAsync(ct).ConfigureAwait(false);

        //        using var retry = new HttpRequestMessage(method, path)
        //        {
        //            Content = body == null ? null : new StringContent(JsonConvert.SerializeObject(body, _jsonSettings), Encoding.UTF8, "application/json")
        //        };
        //        return await _http.SendAsync(retry, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        //    }

        //    return res;
        //}



        // ============================ DTOs ============================

        private sealed class InnerSapEnvelope { public InnerError error { get; set; } }
        private sealed class InnerError
        {
            public string? code { get; set; }
            public string? message { get; set; }
            public InnerDetail[]? details { get; set; }
        }
        private sealed class InnerDetail { public string? code { get; set; } public string? message { get; set; } }
        private sealed class LoginResponse
        {
            public string? SessionId { get; set; }
            public int SessionTimeout { get; set; } // minutes
        }

        private sealed class SlErrorEnvelope { public SlError error { get; set; } }
        private sealed class SlError
        {
            public object code { get; set; }
            public SlMessage message { get; set; }
        }
        private sealed class SlMessage
        {
            public string? lang { get; set; }
            public string? value { get; set; }
        }
    }
}
