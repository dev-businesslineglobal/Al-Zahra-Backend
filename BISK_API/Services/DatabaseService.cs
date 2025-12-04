using GardeningAPI.Data;
using GardeningAPI.Application.Interfaces;
using System.Threading.Tasks;
using GardeningAPI.Model;

namespace BISK_API.Services
{
    public class DatabaseService : IDatabase
    {
        private readonly OdbcClient _connection;

        public DatabaseService(OdbcClient connection)
        {
            _connection = connection;
        }

        // ==================== User Related Methods =================== //
        #region User Related Methods
  
        public Task<bool> CheckUserVerificationStatus(string email)
        {
            return _connection.CheckUserVerificationStatus(email);
        }
        public async Task<SignUpResponse?> GetLoginDetailsSignUp(string email)
        {
            return await _connection.GetLoginDetailsAsync(email);
        }
        public Task<UserDetails> GetUserDetails(string email)
        {
            return _connection.GetUserDetailsForOTP(email);
        }
        public async Task<bool> ValidateEmailAsync(string email)
        {
            return await _connection.ValidateEmailAsync(email);
        }
        public async Task<bool> ValidatePasswordAsync(string email, string password)
        {
            return await _connection.ValidatePasswordAsync(email, password);
        }
        public async Task<bool> ValidateUsernameAsync(string username)
        {
            return await _connection.ValidateUsernameAsync(username);
        }

        #endregion

        // =================== OTP Related Methods =================== //
        #region OTP Related Methods

        public async Task<UserDetails> GetUserForOTPSetup(string email)
        {
            return await _connection.GetUserDetailsForOTP(email);
        }
        public Task<OTP?> GetOtpRecordAsync(string CardCode)
            => _connection.GetOtpRecordAsync(CardCode);
        public async Task<string> GetCardCodeByEmailAsync(string email)
        {
            return await _connection.GetCardCodeByEmailAsync(email) ?? string.Empty;
        }

        #endregion

        // ==================== Item Related Methods =================== //
        public Task<List<ItemsMasterData>> GetItems(string whsCode)
        {
            return _connection.GetItems(whsCode);
        }
        public async Task<List<Cart>> GetCartDetails(string CardCode)
        {
            var result = await _connection.GetCartDetails(CardCode);
            return result ?? new List<Cart>();
        }

        public async Task<Cart> GetSingleCartDetails(int docEntry)
        {
            var result = await _connection.GetSingleCartFromDB(docEntry);
            return result;
        }


        // ==================== Other Methods =================== //


        // ==================== End of Class =================== //
    }
}
