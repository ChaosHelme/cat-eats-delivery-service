using System.Text.Json;
using CatEats.Domain.ValueObjects;
using CatEats.UserService.Application.DTOs;
using CatEats.UserService.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;

namespace CatEats.UserService.Application;

public class UserApplicationService(
    IUserRepository userRepository,
    IDistributedCache cache)
    : IUserApplicationService
{
    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        return user != null ? UserDto.FromDomain(user) : null;
    }

    public async Task<IEnumerable<UserDto>> GetAvailableRidersAsync(CancellationToken cancellationToken = default)
    {
        var riders = await userRepository.GetRidersAsync(cancellationToken);
        return riders.Select(UserDto.FromDomain);
    }

    public async Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(new UserId(userId), cancellationToken);
        if (user == null) return;

        user.UpdateLastLogin();
        await userRepository.UpdateAsync(user, cancellationToken);

        // Clear user cache
        await cache.RemoveAsync($"user:{userId}", cancellationToken);
    }
}