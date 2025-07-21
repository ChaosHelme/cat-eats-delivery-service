namespace CatEats.Domain.ValueObjects;

public class Address
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public bool IsDefault { get; private set; }

    private Address() { } // EF Constructor

    public Address(string street, string city, string postalCode, string country, 
                  double latitude, double longitude, bool isDefault = false)
    {
        Street = ValidateAndSetString(street, nameof(street));
        City = ValidateAndSetString(city, nameof(city));
        PostalCode = ValidateAndSetString(postalCode, nameof(postalCode));
        Country = ValidateAndSetString(country, nameof(country));
        Latitude = ValidateLatitude(latitude);
        Longitude = ValidateLongitude(longitude);
        IsDefault = isDefault;
    }

    public void SetAsDefault() => IsDefault = true;
    public void SetAsNonDefault() => IsDefault = false;

    private static string ValidateAndSetString(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} cannot be empty", fieldName);
        return value.Trim();
    }

    private static double ValidateLatitude(double latitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
        return latitude;
    }

    private static double ValidateLongitude(double longitude)
    {
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));
        return longitude;
    }
}