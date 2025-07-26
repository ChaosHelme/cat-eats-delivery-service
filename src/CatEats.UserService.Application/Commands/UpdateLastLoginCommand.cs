using CatEats.UserService.Infrastructure;

namespace CatEats.UserService.Application.Commands;

public record UpdateLastLoginCommand(Guid UserId) : ICommand;