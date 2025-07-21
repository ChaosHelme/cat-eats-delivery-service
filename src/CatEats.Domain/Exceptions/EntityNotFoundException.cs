namespace CatEats.Domain.Exceptions;

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityType, string identifier) 
        : base($"{entityType} with identifier '{identifier}' was not found.") { }
}