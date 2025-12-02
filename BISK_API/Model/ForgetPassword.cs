namespace GardeningAPI.Model
{

    public class ForgetPasswordRequest
    {
        public string? Email { get; set; }
    }

    public class VerifyOtpRequest
    {
        public string? Email { get; set; }
        public string? OTP { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string? Email { get; set; }
        public string? NewPassword { get; set; }
    }


    public class UserDetails
    {
        public string? CardCode { get; set; }
        public string? userName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Mobile { get; set; }

    }

    public class OTP
    {
        public string? Code = null;
        public string? U_Email { get; set; }
        public string? U_OTPCode { get; set; }
        public DateTime U_CreatedAt { get; set; }
        public DateTime U_ExpireAt { get; set; }
        public string? U_IsUsed { get; set; } = "N";
    }

    public class ResendOtpRequest
    {
        public string? Email { get; set; }
    }

    public class OtpResult
    {
        public OTP? OtpRecord { get; set; }
        public Exception? Error { get; set; }
    }

    public class OtpVerifyRequest
    {
        public string? Email { get; set; }
        public string? Code { get; set; }
    }

}
