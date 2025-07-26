using System.Reflection;
using CatEats.UserService.Application.Commands.Handlers;
using CatEats.UserService.Application.Queries.Handlers;

namespace CatEats.UserService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            if (!type.IsClass || type.IsAbstract)
                continue;

            var interfaces = type.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                if (!@interface.IsGenericType)
                    continue;

                var definition = @interface.GetGenericTypeDefinition();

                if (definition == typeof(ICommandHandler<>)
                    || definition == typeof(IQueryHandler<,>))
                {
                    services.AddScoped(@interface, type);
                }
            }
        }

        return services;
    }
}