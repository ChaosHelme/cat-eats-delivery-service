using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CatEats.Gateway.Middlewares;

public class UserServiceHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public UserServiceHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var serviceUrl = _configuration["ReverseProxy:Clusters:user-service-cluster:Destinations:user-service:Address"];
            var response = await _httpClient.GetAsync($"{serviceUrl}health", cancellationToken);

            return response.IsSuccessStatusCode 
                ? HealthCheckResult.Healthy("User service is healthy") 
                : HealthCheckResult.Unhealthy($"User service returned {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("User service is unavailable", ex);
        }
    }
}