using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CatEats.Gateway.Middlewares;

public class RestaurantServiceHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public RestaurantServiceHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
            var serviceUrl = _configuration["ReverseProxy:Clusters:restaurant-service-cluster:Destinations:restaurant-service:Address"];
            var response = await _httpClient.GetAsync($"{serviceUrl}health", cancellationToken);

            return response.IsSuccessStatusCode 
                ? HealthCheckResult.Healthy("Restaurant service is healthy") 
                : HealthCheckResult.Unhealthy($"Restaurant service returned {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Restaurant service is unavailable", ex);
        }
    }
}