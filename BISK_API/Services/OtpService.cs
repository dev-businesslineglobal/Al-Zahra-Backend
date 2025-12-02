//using GardeningAPI.Application.Interfaces;
//using GardeningAPI.Model;
//using System.Globalization;

//namespace GardeningAPI.Services
//{
//    public class OtpService
//    {
//        private readonly IDatabase _db;
//        private readonly IEmailService _email;
//        private readonly IPostingBusinessLogic _post;

//        //public OtpService(IDatabase db, IEmailService email, IPostingBusinessLogic post)
//        //{
//        //    _db = db;
//        //    _email = email;
//        //    _post = post;
//        //}

//        //public string GenerateOtp()
//        //{
//        //    return new Random().Next(100000, 999999).ToString();
//        //}


//        //public async Task<OTP> SendOtpAsync(string email)
//        //{
//        //    try
//        //    {
//        //        var otp = GenerateOtp();
//        //        var user = await _db.GetUserForOTPSetup(email);

//        //        var otpRecord = new OTP
//        //        {
//        //            Code = user.CardCode,
//        //            U_Email = email,
//        //            U_OTPCode = otp,
//        //            U_CreatedAt = DateTime.UtcNow,
//        //            U_ExpireAt = DateTime.UtcNow.AddMinutes(10),
//        //            U_IsUsed = "N",
//        //        };

//        //        await _email.SendEmailAsync(email, "OTP Verification", $"Your OTP is {otp}");

//        //        return otpRecord;
//        //        //return new OtpResult{
//        //        //    OtpRecord = otpRecord,
//        //        //    Error = null
//        //        //};
//        //    }
//        //    catch (Exception)
//        //    {
//        //        return null;
//        //        //return new OtpResult
//        //        //{
//        //        //    OtpRecord = null,
//        //        //    Error = ex
//        //        //};
//        //    }
//        //}

//        //public async Task<bool> SendOtpAsync(string email)
//        //{
//        //    try
//        //    {
//        //        var otp = GenerateOtp();
//        //        var user = await _db.GetUserForOTPSetup(email);

//        //        var otpRecord = new OTP
//        //        {
//        //            Code = user.CardCode,
//        //            U_Email = email,
//        //            U_OTPCode = otp,
//        //            U_CreatedAt = DateTime.UtcNow,
//        //            U_ExpireAt = DateTime.UtcNow.AddMinutes(10),
//        //            U_IsUsed = "N",
//        //        };
//        //        var saved = await _post.SaveOTP(otpRecord);

//        //        if (saved.StatusCode == 400 || saved == null) return false;

//        //        await _email.SendEmailAsync(email, "OTP Verification", $"Your OTP is {otp}");

//        //        return true;
//        //    }
//        //    catch(Exception ex)
//        //    {
//        //        // Log the exception (ex) as needed
//        //        Console.WriteLine(ex.ToString());
//        //        return false;
//        //    }
//        //}
//    }

//}
