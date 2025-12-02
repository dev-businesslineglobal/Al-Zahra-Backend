using SAPbobsCOM;
using gardnerAPIs.Common;
using GardeningAPI.Model;

//namespace gardnerAPIs.Application.Interfaces
namespace GardeningAPI.Application.Interfaces
{
    public interface IDatabase
    {
        // ------------------- Interface User Related Methods ------------------ //
        #region User Related Methods
        Task<bool> ValidateEmailAsync(string email);
        Task<bool> ValidatePasswordAsync(string cardCode, string password);
        Task<UserDetails> GetUserDetails(string email);
        Task<SignUpResponse?> GetLoginDetailsSignUp(string email);
        Task<UserDetails> GetUserForOTPSetup(string email);

        #endregion

        // ------------------- Interface OTP Related Methods ------------------ //
        #region OTP Related Methods
        Task<OTP?> GetOtpRecordAsync(string CardCode);
        Task<bool> CheckUserVerificationStatus(string email);
        //Task<bool> UpdateOTPStatus(string cardCode, string status, string otpCode);
        Task<string> GetCardCodeByEmailAsync(string email);

        #endregion

        #region Items
        Task<List<ItemsMasterData>> GetItems(string whsCode);
        Task<Cart?> GetCartDetails(string CardCode);

        #endregion
    }
}
