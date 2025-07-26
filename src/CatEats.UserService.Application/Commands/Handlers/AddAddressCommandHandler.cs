using CatEats.Domain.ValueObjects;
using CatEats.UserService.Infrastructure;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Caching.Distributed;

namespace CatEats.UserService.Application.Commands.Handlers;

public class AddAddressCommandHandler(IUserRepository userRepository, IPublishEndpoint publishEndpoint, IDistributedCache cache) : ICommandHandler<AddAddressCommand>
{
    public async ValueTask<Unit> Handle(AddAddressCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(new UserId(command.UserId), cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {command.UserId} not found");
        }

        user.AddAddress(command.Street, command.City, command.PostalCode, command.Country,
            command.Latitude, command.Longitude, command.IsDefault);

        await userRepository.UpdateAsync(user, cancellationToken);

        // Publish domain events
        foreach (var domainEvent in user.DomainEvents)
        {
            await publishEndpoint.Publish(domainEvent, cancellationToken);
        }

        // Clear user cache
        await cache.RemoveAsync($"user:{command.UserId}", cancellationToken);
        
        return Unit.Value;
    }
}