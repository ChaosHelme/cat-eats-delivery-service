using CatEats.Domain.ValueObjects;
using CatEats.UserService.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace CatEats.UserService.Infrastructure.Persistence;

[PrimaryKey(nameof(Email))]
public record UserEntity
{
    public required UserId Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public UserRole Role { get; init; }
    public UserStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public IList<Address> Addresses { get; init; } = new List<Address>();
}