using System.Diagnostics;

namespace CatEats.Gateway.Middlewares;

public class RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;
        
        logger.LogInformation("Gateway Request Started: {Method} {Path} | RequestId: {RequestId}",
            context.Request.Method, context.Request.Path, requestId);

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("Gateway Request Completed: {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | RequestId: {RequestId}",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, 
                stopwatch.ElapsedMilliseconds, requestId);
        }
    }
}