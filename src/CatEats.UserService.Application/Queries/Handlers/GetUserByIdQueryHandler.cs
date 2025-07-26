using System.Text.Json;
using CatEats.Domain.ValueObjects;
using CatEats.UserService.Application.DTOs;
using CatEats.UserService.Infrastructure;
using Mediator;
using Microsoft.Extensions.Caching.Distributed;

namespace CatEats.UserService.Application.Queries.Handlers;

public class GetUserByIdQueryHandler(IUserRepository userRepository, IDistributedCache cache) : IQueryHandler<GetUserByIdQuery, UserDto?>
{
    public async ValueTask<UserDto?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"user:{query.Id}";
        var cachedUser = await cache.GetStringAsync(cacheKey, cancellationToken);
        
        if (cachedUser != null)
        {
            return JsonSerializer.Deserialize<UserDto>(cachedUser);
        }

        var user = await userRepository.GetByIdAsync(new UserId(query.Id), cancellationToken);
        if (user == null) return null;

        var userDto = UserDto.FromDomain(user);
        
        // Cache for 5 minutes
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(userDto), cacheOptions, cancellationToken);

        return userDto;
    }
}