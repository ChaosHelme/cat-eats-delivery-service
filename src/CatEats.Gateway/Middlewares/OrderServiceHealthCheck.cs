using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CatEats.Gateway.Middlewares;

public class OrderServiceHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OrderServiceHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
            var serviceUrl = _configuration["ReverseProxy:Clusters:order-service-cluster:Destinations:order-service:Address"];
            var response = await _httpClient.GetAsync($"{serviceUrl}health", cancellationToken);

            return response.IsSuccessStatusCode 
                ? HealthCheckResult.Healthy("Order service is healthy") 
                : HealthCheckResult.Unhealthy($"Order service returned {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Order service is unavailable", ex);
        }
    }
}