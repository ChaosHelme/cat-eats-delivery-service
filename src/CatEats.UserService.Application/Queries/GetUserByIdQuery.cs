using CatEats.UserService.Application.DTOs;
using Mediator;

namespace CatEats.UserService.Application.Queries;

public record GetUserByIdQuery(Guid Id) : IQuery<UserDto?>;