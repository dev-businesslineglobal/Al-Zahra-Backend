using System.Collections.Generic;

//namespace gardnerAPIs.Application.Interfaces
namespace GardeningAPI.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username, Dictionary<string, string>? extraClaims = null);
    }
}
