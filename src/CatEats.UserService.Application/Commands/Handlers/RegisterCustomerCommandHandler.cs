using CatEats.UserService.Application.DTOs;
using CatEats.UserService.Domain.Aggregates;
using CatEats.UserService.Infrastructure;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CatEats.UserService.Application.Commands.Handlers;

public class RegisterCustomerCommandHandler(
    IUserRepository userRepository,
    IPublishEndpoint publishEndpoint,
    ILogger<RegisterCustomerCommand> logger) : ICommandHandler<RegisterCustomerCommand, UserDto>
{
    public async Task<UserDto> Handle(RegisterCustomerCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering customer with email {Email}", command.Email);

        if (await userRepository.ExistsAsync(command.Email, cancellationToken))
        {
            throw new InvalidOperationException($"User with email {command.Email} already exists");
        }

        var user = User.CreateCustomer(command.Email, command.FirstName, command.LastName, command.PhoneNumber);
        
        await userRepository.AddAsync(user, cancellationToken);

        // Publish domain events
        foreach (var domainEvent in user.DomainEvents)
        {
            await publishEndpoint.Publish(domainEvent, cancellationToken);
        }

        logger.LogInformation("Customer registered successfully with ID {UserId}", user.Id);
        
        return UserDto.FromDomain(user);
    }
}