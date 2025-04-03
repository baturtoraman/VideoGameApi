using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using VideoGameApi.Responses;
using VideoGameApi.Constants;

namespace VideoGameApi.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Request: {Method} {Path} {QueryString}",
                    context.Request.Method, context.Request.Path, context.Request.QueryString);

                var originalBodyStream = context.Response.Body;
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await _next(context);

                responseBody.Seek(0, SeekOrigin.Begin);
                string responseText = await new StreamReader(responseBody).ReadToEndAsync();

                _logger.LogInformation("Response Status: {StatusCode}, Body: {Body}",
                    context.Response.StatusCode, responseText);

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                var errorResponse = new ResponseModel<string>(
                    success: false,
                    message: ResponseMessages.InternalServerError
                );

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
        }
    }
}
