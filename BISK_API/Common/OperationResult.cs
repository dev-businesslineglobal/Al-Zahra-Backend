using System.Collections.Generic;

namespace gardnerAPIs.Common
{
    /// <summary>
    /// Generic operation envelope used by business/service layers.
    /// </summary>
    public class OperationResult<T>
    {
        /// <summary>True when the operation succeeded end-to-end.</summary>
        public bool IsSuccess { get; set; }

        /// <summary>Successful result payload (null if IsSuccess = false).</summary>
        public T Result { get; set; }

        /// <summary>One or more errors when IsSuccess = false (normalized, human-friendly).</summary>
        public List<ErrorResponse> ErrorMessages { get; set; } = new();

        /// <summary>Optional correlation/tracing id if you want to set it.</summary>
        public string CorrelationId { get; set; }

        /// <summary>Optional HTTP status associated with this operation (if applicable).</summary>
        public int? HttpStatus { get; set; }
    }

    /// <summary>
    /// Normalized error block. Backward-compatible with your previous structure,
    /// but extended with friendly fields.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>Legacy/compatible holder (code + message.value).</summary>
        public ErrorDetails error { get; set; } = new();

        /// <summary>High-level source (e.g., "ServiceLayer", "HttpClient", "ODBC").</summary>
        public string source { get; set; } = "ServiceLayer";

        /// <summary>HTTP status code if known (e.g., 400, 401, 500).</summary>
        public int? httpStatus { get; set; }

        /// <summary>Optional list of granular error items (from SL 'details').</summary>
        public List<ErrorDetailItem> details { get; set; } = new();

        /// <summary>The raw payload we parsed (only for diagnostics).</summary>
        public string raw { get; set; }
    }

    /// <summary>Legacy/compatible shell used by your existing clients.</summary>
    public class ErrorDetails
    {
        /// <summary>Service-layer code (or fallback HTTP code).</summary>
        public int code { get; set; }

        /// <summary>Localized message object (lang + value).</summary>
        public ErrorMessage message { get; set; } = new();
    }

    /// <summary>Localized message.</summary>
    public class ErrorMessage
    {
        public string lang { get; set; } = "en-us";
        public string value { get; set; }
    }

    /// <summary>Single detail row, when SL provides an array in the error.</summary>
    public class ErrorDetailItem
    {
        public string code { get; set; }
        public string message { get; set; }
    }
}
