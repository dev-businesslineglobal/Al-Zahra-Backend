using Dapper;
using GardeningAPI.Application.Interfaces;
using GardeningAPI.Model;

namespace gardnerAPIs.Services
{
    public class AuthService : IUserRepository
    {
        private readonly IDatabase _db;
        private readonly IEmailService _email;
        public AuthService(IDatabase db,IEmailService email)
        {
            _db = db;
            _email = email;
        }

        public async Task<SignUpResponse?> ValidateCredentialsAsync(string email, string passwordHash)
        {
            if (await _db.ValidatePasswordAsync(email, passwordHash))
            {
                return await _db.GetLoginDetailsSignUp(email);
            }

            return null;
        }

        public string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }
        public async Task<OtpResult> SendOtpAsync(string email)
        {
            try
            {
                var otp = GenerateOtp();
                var user = await _db.GetUserForOTPSetup(email);
                
                if (user == null)
                {
                    throw new InvalidOperationException("User not found.");
                }
                var otpRecord = new OTP
                {
                    Code = user.CardCode,
                    U_Email = email,
                    U_OTPCode = otp,
                    U_CreatedAt = DateTime.UtcNow,
                    U_ExpireAt = DateTime.UtcNow.AddMinutes(10),
                    U_IsUsed = "N",
                };

                await _email.SendEmailAsync(email, "OTP Verification", $"Your OTP is {otp}");

                return new OtpResult
                {
                    OtpRecord = otpRecord,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new OtpResult
                {
                    OtpRecord = null,
                    Error = ex
                };
            }
        }
    }
}
