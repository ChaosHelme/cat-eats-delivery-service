using CatEats.UserService.Domain.Aggregates;
using CatEats.UserService.Domain.Enumerations;

namespace CatEats.UserService.Application.DTOs;

public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public UserStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public List<AddressDto> Addresses { get; init; } = [];

    public static UserDto FromDomain(User user)
    {
        return new UserDto
        {
            Id = user.Id.Value,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Addresses = user.Addresses.Select(AddressDto.FromDomain).ToList()
        };
    }
}