using CatEats.UserService.Application.DTOs;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using CatEats.Domain.AggregateRoots;
using CatEats.Domain.ValueObjects;
using CatEats.UserService.Application;
using CatEats.UserService.Application.Commands;
using Microsoft.Extensions.Logging;

namespace FoodDelivery.UserService.Application;

public class UserApplicationService : IUserApplicationService
{
    private readonly IUserRepository _userRepository;
    private readonly IDistributedCache _cache;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<UserApplicationService> _logger;

    public UserApplicationService(
        IUserRepository userRepository,
        IDistributedCache cache,
        IPublishEndpoint publishEndpoint,
        ILogger<UserApplicationService> logger)
    {
        _userRepository = userRepository;
        _cache = cache;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<UserDto> RegisterCustomerAsync(RegisterCustomerCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Registering customer with email {Email}", command.Email);

        if (await _userRepository.ExistsAsync(command.Email, cancellationToken))
        {
            throw new InvalidOperationException($"User with email {command.Email} already exists");
        }

        var user = User.CreateCustomer(command.Email, command.FirstName, command.LastName, command.PhoneNumber);
        
        await _userRepository.AddAsync(user, cancellationToken);

        // Publish domain events
        foreach (var domainEvent in user.DomainEvents)
        {
            await _publishEndpoint.Publish(domainEvent, cancellationToken);
        }

        _logger.LogInformation("Customer registered successfully with ID {UserId}", user.Id);

        return UserDto.FromDomain(user);
    }

    public async Task<UserDto> RegisterRiderAsync(RegisterRiderCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Registering rider with email {Email}", command.Email);

        if (await _userRepository.ExistsAsync(command.Email, cancellationToken))
        {
            throw new InvalidOperationException($"User with email {command.Email} already exists");
        }

        var user = User.CreateRider(command.Email, command.FirstName, command.LastName, command.PhoneNumber);
        
        await _userRepository.AddAsync(user, cancellationToken);

        // Publish domain events
        foreach (var domainEvent in user.DomainEvents)
        {
            await _publishEndpoint.Publish(domainEvent, cancellationToken);
        }

        _logger.LogInformation("Rider registered successfully with ID {UserId}", user.Id);

        return UserDto.FromDomain(user);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user:{id}";
        var cachedUser = await _cache.GetStringAsync(cacheKey, cancellationToken);
        
        if (cachedUser != null)
        {
            return JsonSerializer.Deserialize<UserDto>(cachedUser);
        }

        var user = await _userRepository.GetByIdAsync(new UserId(id), cancellationToken);
        if (user == null) return null;

        var userDto = UserDto.FromDomain(user);
        
        // Cache for 5 minutes
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(userDto), cacheOptions, cancellationToken);

        return userDto;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return user != null ? UserDto.FromDomain(user) : null;
    }

    public async Task<IEnumerable<UserDto>> GetAvailableRidersAsync(CancellationToken cancellationToken = default)
    {
        var riders = await _userRepository.GetRidersAsync(cancellationToken);
        return riders.Select(UserDto.FromDomain);
    }

    public async Task AddAddressAsync(Guid userId, AddAddressCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(userId), cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        user.AddAddress(command.Street, command.City, command.PostalCode, command.Country,
                       command.Latitude, command.Longitude, command.IsDefault);

        await _userRepository.UpdateAsync(user, cancellationToken);

        // Publish domain events
        foreach (var domainEvent in user.DomainEvents)
        {
            await _publishEndpoint.Publish(domainEvent, cancellationToken);
        }

        // Clear user cache
        await _cache.RemoveAsync($"user:{userId}", cancellationToken);
    }

    public async Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(userId), cancellationToken);
        if (user == null) return;

        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Clear user cache
        await _cache.RemoveAsync($"user:{userId}", cancellationToken);
    }
}