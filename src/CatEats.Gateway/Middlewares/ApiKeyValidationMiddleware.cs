namespace CatEats.Gateway.Middlewares;

public class ApiKeyValidationMiddleware(IConfiguration configuration, ILogger<ApiKeyValidationMiddleware> logger)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Skip API key validation for health checks and auth endpoints
        if (context.Request.Path.StartsWithSegments("/health") || 
            context.Request.Path.StartsWithSegments("/api/auth"))
        {
            await next(context);
            return;
        }

        // Check for API key in header
        if (!context.Request.Headers.TryGetValue("X-API-Key", out var extractedApiKey))
        {
            // If no JWT token either, require API key
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key missing");
                return;
            }
        }
        else
        {
            var validApiKeys = configuration.GetSection("ApiKeys").Get<string[]>() ?? [];
            if (!Array.Exists(validApiKeys, element => element == extractedApiKey))
            {
                logger.LogWarning("Invalid API Key attempted: {ApiKey} from {RemoteIp}", 
                    extractedApiKey, context.Connection.RemoteIpAddress);
                
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            logger.LogDebug("Valid API Key used: {ApiKey}", extractedApiKey);
        }

        await next(context);
    }
}