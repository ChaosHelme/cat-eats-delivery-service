using CatEats.UserService.Application.DTOs;
using Mediator;

namespace CatEats.UserService.Application.Commands;

public record RegisterRiderCommand(string Email, string FirstName, string LastName, string PhoneNumber) : ICommand<UserDto>;