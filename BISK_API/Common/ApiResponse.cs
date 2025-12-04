namespace gardnerAPIs.Common
{
    public class ApiResponse
    {
        public bool success { get; set; }
        public object? data { get; set; }
        public string? error { get; set; }
        public int? DocEntry { get; set; }
    }
}
