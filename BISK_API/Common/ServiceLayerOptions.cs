namespace BISK_API.Services
{
    public sealed class ServiceLayerOptions
    {
        public string Url { get; set; } = "";       // e.g., https://192.168.40.180:50000/b1s/v2/
        public string CompanyDB { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public int SessionTtlMinutes { get; set; } = 30;

        // DEV-ONLY: bypass TLS validation (self-signed cert etc.)
        public bool SkipTlsVerify { get; set; } = false;
    }
}
