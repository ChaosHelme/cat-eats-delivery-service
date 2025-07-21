namespace CatEats.Domain.ValueObjects;

public record LocationUpdate
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string? Address { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private LocationUpdate() { } // EF Constructor

    public LocationUpdate(double latitude, double longitude, string? address = null)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        Latitude = latitude;
        Longitude = longitude;
        Address = address?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}