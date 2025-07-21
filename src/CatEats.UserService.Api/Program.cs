using CatEats.UserService.Application;
using CatEats.UserService.Infrastructure;
using FoodDelivery.UserService.Application;
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
        builder.Services.AddScoped<IUserRepository, UserRepository>();

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

        // Add Redis caching
        builder.AddRedisClient("redis");

        // Add controllers
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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