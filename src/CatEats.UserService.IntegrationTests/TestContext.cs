using CatEats.UserService.Api;
using CatEats.UserService.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace CatEats.UserService.IntegrationTests;

public class TestContext : IAsyncDisposable
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly RedisContainer _redisContainer;
    private readonly RabbitMqContainer _rabbitMqContainer;
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _httpClient;

    public TestContext()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();

        _redisContainer = new RedisBuilder()
            .Build();

        _rabbitMqContainer = new RabbitMqBuilder()
            .Build();
    }

    public HttpClient HttpClient => _httpClient ??= CreateHttpClient();

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
    }

    private HttpClient CreateHttpClient()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UserDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add test database context
                    services.AddDbContext<UserDbContext>(options =>
                    {
                        options.UseNpgsql(_postgresContainer.GetConnectionString());
                    });

                    // Override Redis connection
                    // services.Configure<Microsoft.Extensions.Caching.StackExchangeRedis.RedisCacheOptions>(options =>
                    // {
                    //     options.Configuration = _redisContainer.GetConnectionString();
                    // });
                    //
                    // // Override RabbitMQ connection
                    // services.PostConfigure<MassTransit.RabbitMqTransportOptions>(options =>
                    // {
                    //     options.Host = _rabbitMqContainer.Hostname;
                    //     options.Port = (ushort)_rabbitMqContainer.GetMappedPublicPort(5672);
                    //     options.Username = "guest";
                    //     options.Password = "guest";
                    // });
                });

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Warning);
                });
            });

        return _factory.CreateClient();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = _factory!.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _httpClient?.Dispose();
        _factory?.Dispose();
        
        await _postgresContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
        await _rabbitMqContainer.DisposeAsync();
    }
}