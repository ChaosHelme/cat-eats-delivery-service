namespace CatEats.Domain.ValueObjects;

public record Location
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string? Address { get; private set; }

    public Location(double latitude, double longitude, string? address = null)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        Latitude = latitude;
        Longitude = longitude;
        Address = address?.Trim();
    }

    public double DistanceToKm(Location other)
    {
        const double earthRadiusKm = 6371.0;
        
        var lat1Rad = Latitude * Math.PI / 180.0;
        var lon1Rad = Longitude * Math.PI / 180.0;
        var lat2Rad = other.Latitude * Math.PI / 180.0;
        var lon2Rad = other.Longitude * Math.PI / 180.0;
        
        var deltaLat = lat2Rad - lat1Rad;
        var deltaLon = lon2Rad - lon1Rad;
        
        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
            Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
            Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return earthRadiusKm * c;
    }
}