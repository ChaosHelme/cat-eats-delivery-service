using CatEats.UserService.Application.DTOs;
using CatEats.UserService.Domain.Aggregates;
using CatEats.UserService.Infrastructure;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CatEats.UserService.Application.Commands.Handlers;

public class RegisterRiderCommandHandler(IUserRepository userRepository, IPublishEndpoint publishEndpoint, ILogger<RegisterCustomerCommand> logger) : ICommandHandler<RegisterRiderCommand, UserDto>
{
    public async Task<UserDto> Handle(RegisterRiderCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering rider with email {Email}", command.Email);

        if (await userRepository.ExistsAsync(command.Email, cancellationToken))
        {
            throw new InvalidOperationException($"User with email {command.Email} already exists");
        }

        var user = User.CreateRider(command.Email, command.FirstName, command.LastName, command.PhoneNumber);

        await userRepository.AddAsync(user, cancellationToken);

        // Publish domain events
        foreach (var domainEvent in user.DomainEvents)
        {
            await publishEndpoint.Publish(domainEvent, cancellationToken);
        }

        logger.LogInformation("Rider registered successfully with ID {UserId}", user.Id);

        return UserDto.FromDomain(user);
    }
}