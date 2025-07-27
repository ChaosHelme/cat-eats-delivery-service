using CatEats.UserService.Application.Queries.Handlers;

namespace CatEats.UserService.Application.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public class QueryHandlerAttribute : Attribute
{
    public Type HandlerType { get; }

    public QueryHandlerAttribute(Type handlerType)
    {
        if (!IsValidHandlerType(handlerType))
            throw new ArgumentException($"Type {handlerType.Name} must implement IQueryHandler<IQuery, TResponse>.");

        HandlerType = handlerType;
    }

    private static bool IsValidHandlerType(Type type)
    {
        return type.GetInterfaces().Any(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
        );
    }
}