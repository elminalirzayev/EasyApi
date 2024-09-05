using System.Collections.Generic;
using System.Net;

namespace Application.Response
{
    public class BaseResponse
    {
        public bool Succeeded { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public BaseResponse() { }

        public BaseResponse(string message) : this()
        {
            Message = message;
        }

        public BaseResponse(string message, bool success) : this(message)
        {
            Succeeded = success;
            StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
        }

        public BaseResponse(string message, bool success, HttpStatusCode statusCode) : this(message, success)
        {
            StatusCode = statusCode;
        }
        public BaseResponse(string message, bool success, HttpStatusCode statusCode, List<string> errors) : this(message, success)
        {
            StatusCode = statusCode;
            Errors = errors;
        }

    }
}
