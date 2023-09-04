using System;
using System.Net;
using System.Text.Json;

namespace RealStateAPI.Middlewares
{
    public class GlobalErrorHandling
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            } catch(Exception ex) { 
                await HandleInheritability(httpContext, ex);
            }
        }

        private Task HandleInheritability(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError; // 500

            var errorResponse = new
            {
                Message = "An error occurred while processing your request.",
                ExceptionMessage = ex.Message
            };

            return httpContext.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }

    public static class GlobalErrorHandlingExtensions {
    
        public static IApplicationBuilder UseGlobalErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalErrorHandling>();
        }
    }

}
