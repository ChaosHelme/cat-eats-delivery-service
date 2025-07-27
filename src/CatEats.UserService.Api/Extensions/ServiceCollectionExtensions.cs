using System.Reflection;
using CatEats.UserService.Application.Attributes;
using CatEats.UserService.Application.Commands.Handlers;
using CatEats.UserService.Application.Queries.Handlers;

namespace CatEats.UserService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var commandHandlerTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<CommandHandlerAttribute>() != null)
            .Select(t => t.GetCustomAttribute<CommandHandlerAttribute>()!.HandlerType)
            .ToList();
        
        var queryHandlerTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<QueryHandlerAttribute>() != null)
            .Select(t => t.GetCustomAttribute<QueryHandlerAttribute>()!.HandlerType)
            .ToList();

        foreach (var type in commandHandlerTypes)
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

        foreach (var type in queryHandlerTypes)
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