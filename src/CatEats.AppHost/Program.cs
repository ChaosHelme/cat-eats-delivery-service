using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure Services
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var userDb = postgres.AddDatabase("userdb");
var restaurantDb = postgres.AddDatabase("restaurantdb");  
var orderDb = postgres.AddDatabase("orderdb");
var deliveryDb = postgres.AddDatabase("deliverydb");

var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithRedisCommander();

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin()
    .WithDataVolume();

var mongo = builder.AddMongoDB("mongodb")
    .WithDataVolume()
    .WithMongoExpress();

var locationDb = mongo.AddDatabase("locationdb");

// API Gateway
var gateway = builder.AddProject<Projects.CatEats_Gateway>("gateway")
    .WithReference(redis)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

// Microservices
var userService = builder.AddProject<Projects.CatEats_UserService_Api>("user-service")
    .WithReference(userDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var restaurantService = builder.AddProject<Projects.CatEats_RestaurantService>("restaurant-service")
    .WithReference(restaurantDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var orderService = builder.AddProject<Projects.CatEats_OrderService>("order-service")
    .WithReference(orderDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var deliveryService = builder.AddProject<Projects.CatEats_DeliveryService>("delivery-service")
    .WithReference(deliveryDb)
    .WithReference(locationDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var notificationService = builder.AddProject<Projects.CatEats_NotificationService>("notification-service")
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

// Gateway references to services
gateway.WithReference(userService)
       .WithReference(restaurantService)
       .WithReference(orderService)
       .WithReference(deliveryService);

// Observability
if (builder.Environment.IsDevelopment())
{
    // Add Seq for centralized logging in development
    var seq = builder.AddSeq("seq");
    
    userService.WithReference(seq);
    restaurantService.WithReference(seq);
    orderService.WithReference(seq);
    deliveryService.WithReference(seq);
    notificationService.WithReference(seq);
    gateway.WithReference(seq);
}

var app = builder.Build();

await app.RunAsync();