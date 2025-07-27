using CatEats.UserService.Application.Attributes;
using CatEats.UserService.Application.Commands.Handlers;

namespace CatEats.UserService.Application.Commands;

[CommandHandler(typeof(AddAddressCommandHandler))]
public record AddAddressCommand(
    Guid UserId, 
    string City, 
    string Street, 
    string Country,
    string PostalCode,
    double Latitude, 
    double Longitude,
    bool IsDefault) : ICommand;