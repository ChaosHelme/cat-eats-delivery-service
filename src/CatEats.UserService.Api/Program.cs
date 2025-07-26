using CatEats.UserService.Application;
using CatEats.UserService.Infrastructure;
using MassTransit;

namespace CatEats.UserService.Api;

partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        // Add Entity Framework
        builder.AddNpgsqlDbContext<UserDbContext>("userdb");

        // Add repositories
        builder.Services.AddScoped<IUserRepository, UserEfCoreRepository>();
        
        // Add application services
        builder.Services.AddScoped<IUserApplicationService, UserApplicationService>();

        // Add MassTransit for messaging
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration.GetConnectionString("messaging"));
                cfg.ConfigureEndpoints(context);
            });
        });

        builder.Services.AddMediator(cfg =>
        {
            cfg.AddConsumers(typeof(UserApplicationService).Assembly);
        });
        
        builder.Services.AddAutoMapper(_ => {}, typeof(UserApplicationService).Assembly);

        // Add Redis caching
        builder.AddRedisClient("redis");

        // Add controllers
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddStackExchangeRedisCache(cfg =>
        {
            cfg.Configuration = builder.Configuration.GetConnectionString("redis");
        });

        // Add health checks
        builder.Services.AddHealthChecks()
            // .AddNpgSql(builder.Configuration.GetConnectionString("userdb")!)
            .AddRedis(builder.Configuration.GetConnectionString("redis")!);

        var app = builder.Build();

        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        // Apply database migrations
        //await app.Services.MigrateDbContextAsync<UserDbContext>();

        app.Run();
    }
}

public partial class Program;