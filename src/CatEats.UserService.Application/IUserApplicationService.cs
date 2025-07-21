using CatEats.UserService.Application.Commands;
using CatEats.UserService.Application.DTOs;

public interface IUserApplicationService
{
    Task<UserDto> RegisterCustomerAsync(RegisterCustomerCommand command, CancellationToken cancellationToken = default);
    Task<UserDto> RegisterRiderAsync(RegisterRiderCommand command, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetAvailableRidersAsync(CancellationToken cancellationToken = default);
    Task AddAddressAsync(Guid userId, AddAddressCommand command, CancellationToken cancellationToken = default);
    Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default);
}