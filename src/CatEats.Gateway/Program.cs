using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using CatEats.Gateway.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add YARP (Yet Another Reverse Proxy)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Authentication & Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Authentication:Authority"],
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("rider-or-admin", policy =>
        policy.RequireAuthenticatedUser());

    options.AddPolicy("authenticated", policy =>
        policy.RequireAuthenticatedUser());
});

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // Global rate limiting
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User?.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    // API-specific rate limiting
    options.AddFixedWindowLimiter("ApiPolicy", o =>
    {
        o.PermitLimit = 50;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 10;
    });

    // Authentication rate limiting  
    options.AddFixedWindowLimiter("AuthPolicy", o =>
    {
        o.PermitLimit = 5;
        o.Window = TimeSpan.FromMinutes(1);
    });
});

// Add caching
builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("redis");
});

// Add health checks for downstream services
builder.Services.AddHealthChecks()
    .AddCheck<UserServiceHealthCheck>("user-service")
    .AddCheck<RestaurantServiceHealthCheck>("restaurant-service")
    .AddCheck<OrderServiceHealthCheck>("order-service")
    .AddCheck<DeliveryServiceHealthCheck>("delivery-service");

// Add custom middleware services
builder.Services.AddScoped<RequestLoggingMiddleware>();
builder.Services.AddScoped<ApiKeyValidationMiddleware>();
builder.Services.AddScoped<RequestResponseLoggingMiddleware>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Use CORS
app.UseCors();

// Use Rate Limiting
app.UseRateLimiter();

// Custom middleware pipeline
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ApiKeyValidationMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Use YARP reverse proxy
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        // Add custom headers
        context.Request.Headers.Add("X-Gateway-Version", "1.0");
        context.Request.Headers.Add("X-Request-Id", context.TraceIdentifier);
        
        await next();
        
        // Log response
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Proxied request to {Destination} with status {StatusCode}",
            context.Request.Path, context.Response.StatusCode);
    });
});

app.Run();

