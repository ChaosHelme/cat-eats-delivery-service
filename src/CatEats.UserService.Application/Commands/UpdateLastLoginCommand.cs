using CatEats.UserService.Application.Attributes;
using CatEats.UserService.Application.Commands.Handlers;

namespace CatEats.UserService.Application.Commands;

[CommandHandler(typeof(UpdateLastLoginCommandHandler))]
public record UpdateLastLoginCommand(Guid UserId) : ICommand;