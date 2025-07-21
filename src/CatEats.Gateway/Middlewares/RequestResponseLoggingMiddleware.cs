using System.Text;

namespace CatEats.Gateway.Middlewares;

public class RequestResponseLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            // Log request details
            await LogRequest(context);
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            // Capture response
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await next(context);

            // Log response details
            await LogResponse(context, responseBody, originalBodyStream);
        }
        else
        {
            await next(context);
        }
    }

    private async Task LogRequest(HttpContext context)
    {
        var request = context.Request;
        
        var requestLog = new StringBuilder();
        requestLog.AppendLine($"Request: {request.Method} {request.Path}{request.QueryString}");
        requestLog.AppendLine($"Host: {request.Host}");
        requestLog.AppendLine($"Content-Type: {request.ContentType}");
        requestLog.AppendLine($"User-Agent: {request.Headers["User-Agent"]}");
        
        if (request.ContentLength > 0 && request.ContentLength < 4096) // Only log small payloads
        {
            request.EnableBuffering();
            var buffer = new byte[request.ContentLength.Value];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var body = Encoding.UTF8.GetString(buffer);
            requestLog.AppendLine($"Body: {body}");
            request.Body.Position = 0; // Reset for downstream processing
        }

        _logger.LogDebug("Gateway Request Details:\n{RequestLog}", requestLog.ToString());
    }

    private async Task LogResponse(HttpContext context, MemoryStream responseBody, Stream originalBodyStream)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);

        var responseLog = new StringBuilder();
        responseLog.AppendLine($"Response: {context.Response.StatusCode}");
        responseLog.AppendLine($"Content-Type: {context.Response.ContentType}");
        
        if (responseBody.Length < 4096) // Only log small responses
        {
            responseLog.AppendLine($"Body: {responseText}");
        }

        _logger.LogDebug("Gateway Response Details:\n{ResponseLog}", responseLog.ToString());

        await responseBody.CopyToAsync(originalBodyStream);
    }
}