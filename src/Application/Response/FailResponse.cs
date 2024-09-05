using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class FailResponse : BaseResponse
    {

        public FailResponse(string message, HttpStatusCode statusCode) : base(message, false, statusCode)
        {

        }
        public FailResponse(string message, HttpStatusCode statusCode, List<string> errors) : base(message, false, statusCode, errors)
        {

        }
    }
}
