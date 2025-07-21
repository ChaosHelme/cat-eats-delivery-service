using CatEats.Domain.AggregateRoots;
using CatEats.Domain.Enumerations;

namespace CatEats.UserService.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<AddressDto> Addresses { get; set; } = new();

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