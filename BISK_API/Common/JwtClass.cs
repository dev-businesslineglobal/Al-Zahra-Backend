using Microsoft.IdentityModel.Tokens;
using GardeningAPI.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PDA_API.Common
{
    public class TokenService
    {

        public static string? SecretKey = ConfigManager.Instance.getConfig()["Jwt:Key"];
        public static string? Issuer = ConfigManager.Instance.getConfig()["Jwt:Issuer"];
        public static string? Audience = ConfigManager.Instance.getConfig()["Jwt:Audeince"];
        public static string? username = ConfigManager.Instance.getConfig()["Jwt:username"];


        public static PostTokenResponse GenerateToken(string username, string session, double sessionTimeout)
        {
            var response = new PostTokenResponse
            {
                Success = true,
                Message = "Token generated successfully.",
                Errors = new List<string>()  
            };

            try
            {
                var key = Encoding.ASCII.GetBytes(SecretKey);

                var tokenHandler = new JwtSecurityTokenHandler();
                var claimsIdentity = new System.Security.Claims.ClaimsIdentity(new[]
                {
            new System.Security.Claims.Claim("sub", username),
            new System.Security.Claims.Claim("session", session)
        });

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claimsIdentity,
                    Issuer = Issuer,
                    Audience = Audience, Expires = DateTime.Now.AddMinutes(sessionTimeout),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                response.Token = tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while generating the token.";
                response.Errors.Add(ex.Message);  // Add error message to list
            }

            return response;
        }

        public class PostTokenResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public string? Token { get; set; } // Add token if successful
            public List<string>? Errors { get; set; } // List to store any errors
        }



        public static bool ValidateToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(SecretKey);
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                handler.ValidateToken(token, parameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false; // Invalid token
            }
        }


        public static string ExtractB1SessionToken(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtTokenObj = tokenHandler.ReadJwtToken(jwtToken);

            // Extract the "b1SessionToken" claim
            var b1SessionToken = jwtTokenObj.Claims
                                            .FirstOrDefault(c => c.Type == "b1SessionToken")?.Value;

            return b1SessionToken;
        }
    }
}
