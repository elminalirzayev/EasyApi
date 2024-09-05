using System.Collections.Generic;
using System.Net;

namespace Application.Response
{
    public class BaseDataResponse<T> : BaseResponse
    {

        public T Data { get; set; }


        public BaseDataResponse(T data, string message = "" , HttpStatusCode statusCode= HttpStatusCode.OK) : base(message, true, statusCode)
        {
            Data = data;
        }
    }
}
