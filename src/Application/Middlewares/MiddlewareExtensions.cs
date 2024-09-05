using Microsoft.AspNetCore.Builder;

namespace Application.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseHandlerMiddleware>();
        }
       
    }
}
