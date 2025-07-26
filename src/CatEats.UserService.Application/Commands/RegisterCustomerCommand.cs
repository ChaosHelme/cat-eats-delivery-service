using CatEats.UserService.Application.DTOs;

namespace CatEats.UserService.Application.Commands;

public record RegisterCustomerCommand(string Email, string FirstName, string LastName, string PhoneNumber)
    : ICommand<UserDto>;