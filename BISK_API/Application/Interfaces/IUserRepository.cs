using GardeningAPI.Model;
using System.Threading.Tasks;



namespace GardeningAPI.Application.Interfaces
{
    public interface IUserRepository
    {

        Task<SignUpResponse?> ValidateCredentialsAsync(string email, string passwordHash);
        Task<OtpResult> SendOtpAsync(string email);
    }
}
