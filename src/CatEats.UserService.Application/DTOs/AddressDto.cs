using CatEats.Domain.ValueObjects;

namespace CatEats.UserService.Application.DTOs;

public record AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsDefault { get; set; }

    public static AddressDto FromDomain(Address address)
    {
        return new AddressDto
        {
            Street = address.Street,
            City = address.City,
            PostalCode = address.PostalCode,
            Country = address.Country,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            IsDefault = address.IsDefault
        };
    }

    public static AddressDto FromEntity(Address arg)
    {
        return new AddressDto
        {
            Street = arg.Street,
            City = arg.City,
            PostalCode = arg.PostalCode,
            Country = arg.Country,
            Latitude = arg.Latitude,
            Longitude = arg.Longitude,
            IsDefault = arg.IsDefault
        };
    }
}