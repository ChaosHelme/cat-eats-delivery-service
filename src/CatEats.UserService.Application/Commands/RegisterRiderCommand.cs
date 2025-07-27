using CatEats.UserService.Application.Attributes;
using CatEats.UserService.Application.Commands.Handlers;
using CatEats.UserService.Application.DTOs;

namespace CatEats.UserService.Application.Commands;

[CommandHandler(typeof(RegisterRiderCommandHandler))]
public record RegisterRiderCommand(string Email, string FirstName, string LastName, string PhoneNumber) : ICommand<UserDto>;