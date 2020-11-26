using System.Collections.Generic;

namespace Application.Products
{
    public record DispenserError
    {
        public DispenserError(string errorCode, IDictionary<string, object>? data = null)
        {
            ErrorCode = errorCode;
            Data = data ?? new Dictionary<string, object>();
        }

        public string ErrorCode { get; }
        public IDictionary<string, object> Data { get; }
    }
}