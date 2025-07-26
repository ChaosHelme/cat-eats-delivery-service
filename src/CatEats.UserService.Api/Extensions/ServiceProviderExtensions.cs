using Microsoft.EntityFrameworkCore;

namespace CatEats.UserService.Api.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task MigrateDbContextAsync<T>(this IServiceProvider serviceProvider)
        where T : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<T>();
        await dbContext.Database.MigrateAsync();
    }
}