using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VideoGameApi.Middleware
{
    public class TimeoutLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TimeoutLoggingMiddleware> _logger;
        private readonly TimeSpan _timeoutDuration = TimeSpan.FromSeconds(5);

        public TimeoutLoggingMiddleware(RequestDelegate next, ILogger<TimeoutLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var timeoutTask = Task.Delay(_timeoutDuration, cancellationToken).ContinueWith(t =>
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogError("Request timed out after 5 seconds. Please try again later.");
                }
            });

            try
            {
                await _next(context);
                cancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during the request: {ex.Message}");
                throw;
            }
            finally
            {
                await timeoutTask;
            }
        }
    }
}
