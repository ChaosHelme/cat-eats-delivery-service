using CatEats.UserService.Application.Attributes;
using CatEats.UserService.Application.DTOs;
using CatEats.UserService.Application.Queries.Handlers;

namespace CatEats.UserService.Application.Queries;

[QueryHandler(typeof(GetAvailableRidersQueryHandler))]
public record GetAvailableRidersQuery : IQuery<IEnumerable<UserDto>>;