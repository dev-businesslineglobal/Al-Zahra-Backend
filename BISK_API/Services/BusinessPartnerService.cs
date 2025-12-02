using gardnerAPIs.Common;
using GardeningAPI.Application.Interfaces;
using GardeningAPI.Model;

namespace GardeningAPI.Services
{
    public class BusinessPartnerService : IBusinessPartnerService
    {
        private readonly IPostingBusinessLogic _post;
        private readonly IDatabase _db;
        private readonly IServiceLayerClient _sl;
        public BusinessPartnerService(IPostingBusinessLogic post, IDatabase db, IServiceLayerClient sl)
        {
            _post = post;
            _db = db;
            _sl = sl;
        }

        // ===================== Implementation of BusinessPartnerService methods goes here =====================
        private int ConvertLanguageToCode(string language)
        {
            return language?.Trim().ToLower() switch
            {
                "english" => 3,
                "arabic" => 32,
                "kurdish" => 40,
                _ => 3   // default English
            };
        }
        public async Task<ApiResult> CreateAsync(SignUpRequest request)
        {
            if (request.EmailAddress == null)
            {
                throw new ArgumentNullException(nameof(request), "Email cannot be null.");
            }
            if (await _db.ValidateEmailAsync(request.EmailAddress))
            {
                throw new InvalidOperationException("Email is already registered.");
            }
            int langCode = ConvertLanguageToCode(request.Language ?? "english");

            var bp = new SignUp
            {
                Series = request.reqSeries,
                CardName = request.CardName,
                EmailAddress = request.EmailAddress,
                LanguageCode = langCode,
                Cellular = request.Cellular,
                U_Password = request.U_Password,
                U_Verified = "N"
            };

            var result = await _post.PostBusinessPartner(bp);

            return result;
        }

        public async Task<bool> UpdateBPAsync(string email, Dictionary<string, object> fields)
        {
            var bp = await _db.GetUserDetails(email);
            if(bp.CardCode == null)
                return false;
            string? cardCode = bp.CardCode;

            if (string.IsNullOrWhiteSpace(cardCode))
                return false;
            if (fields == null || fields.Count == 0)
                return false;

            var result = await _sl.PatchBPU(cardCode, fields);

            return result.IsSuccess;
        }

        public async Task<bool> PostOTP(OTP otpRecord)
        {
            var saved = await _post.SaveOTP(otpRecord);

            if (saved.StatusCode == 400 || saved == null) return false;
            return true;
        }

        public async Task<bool> UpdateOTP(string email, Dictionary<string, object> fields)
        {
            string? Code = _db.GetUserDetails(email).Result.CardCode;
            if (string.IsNullOrWhiteSpace(Code))
                return false;
            var otpRecord = await _db.GetOtpRecordAsync(Code);
            //string cardCode = otpRecord.Code;

            if (string.IsNullOrWhiteSpace(Code))
                return false;
            if (fields == null || fields.Count == 0)
                return false;

            var result = await _sl.PatchOtp(Code, fields);

            return result.IsSuccess;
        }
    }
}
