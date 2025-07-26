using CatEats.Domain.ValueObjects;
using CatEats.UserService.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;

namespace CatEats.UserService.Application.Commands.Handlers;

public class UpdateLastLoginCommandHandler(IUserRepository userRepository, IDistributedCache cache) : ICommandHandler<UpdateLastLoginCommand>
{
    public async Task Handle(UpdateLastLoginCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(new UserId(command.UserId), cancellationToken);
        if (user == null) return;

        user.UpdateLastLogin();
        await userRepository.UpdateAsync(user, cancellationToken);

        // Clear user cache
        await cache.RemoveAsync($"user:{command.UserId}", cancellationToken);
    }
}