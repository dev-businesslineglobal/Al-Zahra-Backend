using GardeningAPI.Model;
using gardnerAPIs.Common;

namespace GardeningAPI.Application.Interfaces
{
    public interface IBusinessPartnerService
    {
        Task<ApiResult> CreateAsync(SignUpRequest request);
        Task<bool> UpdateBPAsync(string email, Dictionary<string, object> fields);
        Task<bool> PostOTP(OTP otpRecord);
        Task<bool> UpdateOTP(string email, Dictionary<string, object> fields);
    }
}
