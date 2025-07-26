using Mediator;

namespace CatEats.UserService.Application.Commands;

public record AddAddressCommand(
    Guid UserId, 
    string City, 
    string Street, 
    string Country,
    string PostalCode,
    double Latitude, 
    double Longitude,
    bool IsDefault) : ICommand;