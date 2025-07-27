using System.Reflection;
using CatEats.UserService.Application.Attributes;
using CatEats.UserService.Application.Commands.Handlers;
using CatEats.UserService.Application.Queries.Handlers;

namespace CatEats.UserService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var types  = assembly.GetTypes();
        
        var commandHandlerTypes = types
            .Select(t => t.GetCustomAttribute<CommandHandlerAttribute>())
            .Where(arg => arg != null)
            .Cast<CommandHandlerAttribute>()
            .ToList();

        var queryHandlerTypes = types
            .Select(t => t.GetCustomAttribute<QueryHandlerAttribute>())
            .Where(arg => arg != null)
            .Cast<QueryHandlerAttribute>()
            .ToList();

        foreach (var type in commandHandlerTypes.Select(t => t.HandlerType))
        {
            if (!type.IsClass || type.IsAbstract)
                throw new InvalidOperationException($"Type {type.FullName} marked with [CommandHandler] must be a non-abstract class.");

            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && (
                    i.GetGenericTypeDefinition().IsAssignableFrom(typeof(ICommandHandler<>)) ||
                    i.GetGenericTypeDefinition().IsAssignableFrom(typeof(ICommandHandler<,>))
                ))
                .ToList();

            if (!interfaces.Any())
                throw new InvalidOperationException($"Type {type.FullName} marked with [CommandHandler] must implement ICommandHandler<ICommand> or ICommandHandler<ICommand, TResponse>");

            foreach (var @interface in interfaces)
            {
                services.AddScoped(@interface, type);
            }
        }

        foreach (var type in queryHandlerTypes.Select(t => t.HandlerType))
        {
            if (!type.IsClass || type.IsAbstract)
                throw new InvalidOperationException($"Type {type.FullName} marked with [QueryHandler] must be a non-abstract class.");

            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition().IsAssignableFrom(typeof(IQueryHandler<,>)))
                .ToList();
            
            if (!interfaces.Any())
                throw new InvalidOperationException($"Type {type.FullName} marked with [QueryHandler] must implement IQueryHandler<,>");

            foreach (var @interface in interfaces)
            {
                services.AddScoped(@interface, type);
            }
        }

        return services;
    }

}