using Application.Exceptions;
using Application.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace Application.Middleware
{
    public class ResponseHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseHandlerMiddleware> _logger;
        public ResponseHandlerMiddleware(RequestDelegate next, ILogger<ResponseHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
                // await ConvertResponse(context);

            }
            catch (Exception ex)
            {
                await ConvertException(context, ex);
            }
        }

        private Task ConvertException(HttpContext context, Exception exception)
        {
            int httpStatusCode;
            var errorMessage = "";
            List<string> errors = new List<string>();

            switch (exception)
            {
                case ModelValidationException validationException:
                    httpStatusCode = (int)HttpStatusCode.BadRequest;
                    errorMessage = "One or more model validation failed. See Errors data.";
                    errors = validationException.ValdationErrors;
                    break;
                case BadRequestException badRequestException:
                    httpStatusCode = (int)HttpStatusCode.BadRequest;
                    errorMessage = badRequestException.Message;
                    break;
                case NotFoundException notFoundException:
                    httpStatusCode = (int)HttpStatusCode.NotFound;
                    errorMessage = notFoundException.Message;
                    break;
                case ApiException apiException:
                    httpStatusCode = (int)HttpStatusCode.InternalServerError;
                    errorMessage = apiException.Message;
                    break;
                default:
                    httpStatusCode = (int)HttpStatusCode.InternalServerError;
                    errorMessage = exception.Message;
                    break;
            }




            context.Response.ContentType = "application/json";
            context.Response.StatusCode = httpStatusCode;


            var result = JsonConvert.SerializeObject(new FailResponse(errorMessage, (HttpStatusCode)httpStatusCode, errors));
            _logger.LogError(result);


            return context.Response.WriteAsync(result);
        }

        private Task ConvertResponse(HttpContext context)
        {
            int httpStatusCode = context.Response.StatusCode;
            var body = context.Response.Body;


            context.Response.ContentType = "application/json";
            context.Response.StatusCode = httpStatusCode;


            var result = JsonConvert.SerializeObject(new BaseDataResponse<dynamic>(body, "", (HttpStatusCode)httpStatusCode));

            _logger.LogError(result);


            return context.Response.WriteAsync(result);
        }
    }
}