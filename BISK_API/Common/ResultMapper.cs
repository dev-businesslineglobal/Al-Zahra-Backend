using SAPbobsCOM;
using System.Linq;

namespace gardnerAPIs.Common
{
    public static class ResultMapper
    {
        public static ApiResult ToApiResult<T>(OperationResult<T> op, object? successData = null)
        {
            var status = 0;
            var message = "An unexpected error occurred.";
            try
            {
                if (op == null)
                    return new ApiResult(500, new ApiResponse { success = false, error = "Unexpected null result" });

                if (op.IsSuccess)
                {
                    // If caller passed a custom successData (e.g., minimal projection), use it
                    //var data = successData ?? op.Result;
                    var data = successData ?? (object?)op.Result;
                    return new ApiResult(op.HttpStatus ?? 200, new ApiResponse
                    {
                        success = true,
                        data = data ?? new { }
                    });
                }

                // Pick the first normalized error message, fall back to generic
                var e = op.ErrorMessages?.FirstOrDefault();
                message = e?.error?.message?.value;
                if (string.IsNullOrWhiteSpace(message)) message = "Error";

                // Choose the best status code
                status = e?.httpStatus ?? op.HttpStatus ?? 400;

                //return new ApiResult(status, new ApiResponse
                //{
                //    success = false,
                //    error = message
                //});
            }
            catch(Exception ex)
            {
                status = 500;
                message = ex.Message;
            }

            return new ApiResult(status, new ApiResponse
            {
                success = false,
                error = message
            });
        }
    }
}
