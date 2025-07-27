using CatEats.UserService.Application.Commands.Handlers;

namespace CatEats.UserService.Application.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class CommandHandlerAttribute : Attribute
{
    public Type HandlerType { get; }

    public CommandHandlerAttribute(Type handlerType)
    {
        if (!IsValidHandlerType(handlerType))
            throw new ArgumentException($"Type {handlerType.Name} must implement ICommandHandler<ICommand> or ICommandHandler<ICommand, TResponse>.");

        HandlerType = handlerType;
    }

    private static bool IsValidHandlerType(Type type)
    {
        return type.GetInterfaces().Any(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
        ) || type.GetInterfaces().Any(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
        );
    }
}