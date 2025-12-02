namespace gardnerAPIs.Common
{
    public sealed class ApiResult
    {
        public int StatusCode { get; }
        public ApiResponse Body { get; }

        public ApiResult(int statusCode, ApiResponse body)
        {
            StatusCode = statusCode;
            Body = body;
        }
    }
}
