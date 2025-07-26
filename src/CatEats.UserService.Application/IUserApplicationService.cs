using CatEats.UserService.Application.Commands;
using CatEats.UserService.Application.DTOs;

public interface IUserApplicationService
{
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetAvailableRidersAsync(CancellationToken cancellationToken = default);
    Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default);
}