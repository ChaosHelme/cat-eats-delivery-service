using CatEats.UserService.Application.DTOs;

namespace CatEats.UserService.Application.Queries;

public record GetAvailableRidersQuery : IQuery<IEnumerable<UserDto>>;