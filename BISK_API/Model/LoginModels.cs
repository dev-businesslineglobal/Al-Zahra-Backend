using GardeningAPI.Model;

namespace GardnerAPI.Model
{
    public class LoginRequest
    {
        public string? email { get; set; }
        public string? Password { get; set; }
    }

    public class LoginResponse
    {
        public string? email { get; set; }
        public string? CardCode { get; set; }
        public string? language { get; set; }
        public string? userName { get; set; }
        public string? mobile { get; set; }
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public string? WhCode { get; set; }
        public string? SessionTimeout { get; set; }
        public BPAddress[]? Address { get; set; }
    }
}