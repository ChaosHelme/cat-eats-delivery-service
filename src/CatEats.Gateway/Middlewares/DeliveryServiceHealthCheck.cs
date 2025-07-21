using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CatEats.Gateway.Middlewares;

public class DeliveryServiceHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public DeliveryServiceHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
            var serviceUrl = _configuration["ReverseProxy:Clusters:delivery-service-cluster:Destinations:delivery-service:Address"];
            var response = await _httpClient.GetAsync($"{serviceUrl}health", cancellationToken);

            return response.IsSuccessStatusCode 
                ? HealthCheckResult.Healthy("Delivery service is healthy") 
                : HealthCheckResult.Unhealthy($"Delivery service returned {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Delivery service is unavailable", ex);
        }
    }
}