using System.Diagnostics;

namespace CatEats.Gateway.Middlewares;

public class RequestLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;
        
        _logger.LogInformation("Gateway Request Started: {Method} {Path} | RequestId: {RequestId}",
            context.Request.Method, context.Request.Path, requestId);

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("Gateway Request Completed: {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | RequestId: {RequestId}",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, 
                stopwatch.ElapsedMilliseconds, requestId);
        }
    }
}