using CatEats.UserService.Application.DTOs;

namespace CatEats.UserService.Application.Queries;

public record GetUserByEmailQuery(string Email) : IQuery<UserDto?>;