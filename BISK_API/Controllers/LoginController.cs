using System.Threading;
using System.Threading.Tasks;
//using gardnerAPIs.Application.Interfaces;
using GardnerAPI.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using GardeningAPI.Model;
using GardeningAPI.Services;
using GardeningAPI.Application.Interfaces;
using GardeningAPI.Data;
using GardeningAPI.Helper;

namespace gardnerAPIs.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [EnableCors("corsapp")]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _auth;
        private readonly IJwtTokenService _jwt;
        private readonly IDatabase _db;
        private readonly IConfig _conf;
        private readonly IEmailService _emailService;
        private readonly IBusinessPartnerService _bpService;
        private readonly string? otp;
        private readonly int expiryMinutes;
        private readonly HelperService _helper;


        public LoginController(IUserRepository auth, IJwtTokenService jwt, IDatabase db, IConfig conf, IEmailService emailService, IBusinessPartnerService bpService, HelperService helper)
        {
            _auth = auth;
            _jwt = jwt;
            _db = db;
            _conf = conf;
            _emailService = emailService;
            var config = ConfigManager.Instance.getConfig();
            otp = config["SignUpOTP:OTP_Code"];
            expiryMinutes = int.TryParse(config["SignUpOTP:Expiry_Minutes"], out var value) ? value : 10;
            _bpService = bpService;
            _helper = helper;
        }

        #region User Related Methods
        // ======================= Login ========================= //

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = "Email and password are required."
                });
            }

            var user = await _auth.ValidateCredentialsAsync(request.email, request.Password);
            if (user == null)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password."
                });
            }
            var verification = await _db.CheckUserVerificationStatus(request.email);
            if (!verification)
            {
                return BadRequest(new
                {
                    Message = "User is not verified.",
                    Code = StatusCode(404)
                });
            }

            var token = _jwt.GenerateToken(request.email);

            var config = await _conf.GetConfiguration();
            var userLanguage = _helper.ConvertCodeToLanguage(user.Language);

            return Ok(new LoginResponse
            {
                email = request.email,
                CardCode = user.CardCode,
                userName = user?.userName ?? "N/A",
                mobile = user?.Mobile ?? "N/A",
                language = userLanguage,
                Success = true,
                Message = "Login successful.",
                Token = token,
                WhCode = config?.whsCode ?? "N/A",
                Address = user?.BPAddresses ?? [],
                SessionTimeout = "60 minutes"

            });
        }

        // ======================= Signup ========================= //

        [HttpPost("Signup")]
        public async Task<IActionResult> Signup([FromBody] SignUpRequest request, CancellationToken ct)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.EmailAddress))
                return BadRequest(new { Success = false, Message = "Email is required." });

            if (await _db.ValidateEmailAsync(request.EmailAddress))
                return BadRequest(new { Success = false, Message = "Email already registered." });

            var config = await _conf.GetConfiguration();
            request.reqSeries = config?.customerSeries;

            var createdUser = await _bpService.CreateAsync(request);
            if (createdUser == null || createdUser.StatusCode == 400)
                return BadRequest(new { Success = false, Message = "Failed to create user." });

            var otpRecord = await _auth.SendOtpAsync(request.EmailAddress);
            if (otpRecord.OtpRecord == null)
            {
                return StatusCode(500, new { Success = false, Message = "Failed to generate OTP. Registration Successful, Just need to verify your account by resending the OTP" });
            }

            bool isSaved = await _bpService.PostOTP(otpRecord.OtpRecord);
            if (!isSaved)
            {
                return StatusCode(500, new { Success = false, Message = "Failed to save OTP. Registration Successful, Just need to verify your account by resending the OTP" });
            }
            var details = await _db.GetLoginDetailsSignUp(request.EmailAddress);

            //await _emailService.SendEmailAsync(request.EmailAddress, "Gardening-APP Login", "You have successfully Registered in to APP!");

            return Ok(new SignUpResponse
            {
                EmailAddress = request.EmailAddress,
                userName = request.CardName,
                CardCode = details?.CardCode ?? "",
                Mobile = request.Cellular,
                Language = request.Language,
                //configuration = config,
                BPAddresses = request.BPAddresses,
                Success = true,
                Message = "Account created. OTP sent. Please verify your account."
            });
        }


        // ======================= Verify Email OTP ========================= //
        [HttpPost("VerifyEmailOtp")]
        public async Task<IActionResult> VerifyEmailOtp([FromBody] OtpVerifyRequest req)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Code))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Email and Code are required."
                    });
                }
                var cardCode = await _db.GetCardCodeByEmailAsync(req.Email);
                if (cardCode == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Email not found."
                    });
                }

                var userOtp = await _db.GetOtpRecordAsync(cardCode);
                if (userOtp == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "OTP not found. Please resend the otp"
                    });
                }

                string correctOtp = userOtp?.U_OTPCode ?? "";
                DateTime expiry = userOtp?.U_ExpireAt ?? DateTime.MinValue;
                string currentVerification = userOtp?.U_IsUsed ?? "Null";

                if (currentVerification == "Y")
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Account already verified Or OTP has been used"
                    });
                }
                if (req.Code != correctOtp)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid OTP."
                    });
                }
                if (expiry < DateTime.UtcNow)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "OTP has expired."
                    });
                }

                var otpStatus = await   _bpService.UpdateOTP(req.Email, new Dictionary<string, object>
                {
                    { "U_IsUsed", "Y" }
                });

                if (!otpStatus)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Verification status failed of OTP."
                    });
                }

                bool statusUpdate = await _bpService.UpdateBPAsync(req.Email, new Dictionary<string, object>
                {
                    { "U_Verified", "Y" }
                });
                if (!statusUpdate)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Verification update failed."
                    });
                }
            
                return Ok(new
                {
                    Success = true,
                    Message = "Account verified successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }

        // ======================= Forget Password ========================= //
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> SendOtp([FromBody] ForgetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { Success = false, Message = "Email is required." });

            var isUserValid = await _db.ValidateEmailAsync(request.Email);
            if (!isUserValid)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email"
                });
            }
            var sent = await _auth.SendOtpAsync(request.Email);
            if (sent.OtpRecord == null)
                return BadRequest(new { Success = false, Message = "Invalid email or not registered." });
            var isSaved = await _bpService.UpdateOTP(request.Email, new Dictionary<string, object>
            {
                { "U_OTPCode", sent.OtpRecord.U_OTPCode ?? "N/A"},
                { "U_CreatedAt", sent.OtpRecord.U_CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ") },
                { "U_ExpireAt", sent.OtpRecord.U_ExpireAt.ToString("yyyy-MM-ddTHH:mm:ssZ") },
                { "U_IsUsed", "N" }
            });
            if (!isSaved)
            {
                return StatusCode(500, new { Success = false, Message = "Could not save OTP." });
            }

            return Ok(new { Success = true, Message = "OTP sent to your email." });
        }

        // ======================= Resend OTP ========================= //

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { Success = false, Message = "Email is required." });

            var isUserValid = await _db.ValidateEmailAsync(request.Email);
            if (!isUserValid)
            {
                return BadRequest(new { Success = false, Message = "User not found." });
            }
            // Check if user already verified
            //bool isVerified = await _db.CheckUserVerificationStatus(request.Email);
            //if (isVerified)
            //{
            //    return BadRequest(new { Success = false, Message = "User already verified." });
            //}

            var sent = await _auth.SendOtpAsync(request.Email);
            if (sent.OtpRecord == null)
            {
                return StatusCode(500, new { Success = false, Message = "Could not generate or Sent OTP. May be technical error in Resending" });
            }
            var isSaved = await _bpService.UpdateOTP(request.Email, new Dictionary<string, object>
            {
                { "U_OTPCode", sent.OtpRecord?.U_OTPCode ?? "N/A" },
                { "U_CreatedAt", sent.OtpRecord.U_CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ") },
                { "U_ExpireAt", sent.OtpRecord.U_ExpireAt.ToString("yyyy-MM-ddTHH:mm:ssZ") },
                { "U_IsUsed", "N" }
            });

            return Ok(new { Success = true, Message = "OTP resent successfully." });
        }

        // ======================= Reset Password ========================= //

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(new { Success = false, Message = "Something is missing. All fields are Mandatory" });

            //var updated = await _auth.UpdateAsync(request.Email, request.NewPassword);
            string cardCode = await _db.GetCardCodeByEmailAsync(request.Email);
            var isOTPVerify = _db.GetOtpRecordAsync(cardCode).Result?.U_IsUsed;
            if(isOTPVerify != "Y")
            {
                return BadRequest(new { Success = false, Message = "OTP is not verified. Cannot reset password." });
            }
            var updated = await _bpService.UpdateBPAsync(request.Email, new Dictionary<string, object>
{
                { "U_Password", request.NewPassword }
            });
            if (!updated)
                return BadRequest(new { Success = false, Message = "Password reset failed. Old password may be incorrect." });

            return Ok(new { Success = true, Message = "Password reset successfully." });
        }

        // ======================= Logout ========================= //

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new
            {
                success = true,
                message = "Logged out successfully. Please remove the token on client side."
            });
        }
        #endregion
    }
}
