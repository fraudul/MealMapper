using SharedKernel;
// ReSharper disable All

namespace Domain.Recommendations;

public sealed record GeoLocation(double Latitude, double Longitude, string City = "Минск")
{
    public double DistanceTo(GeoLocation other)
    {
        const double EarthRadiusMeters = 6371e3;
        double dLat = (other.Latitude - Latitude) * Math.PI / 180;
        double dLon = (other.Longitude - Longitude) * Math.PI / 180;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(Latitude * Math.PI / 180) * Math.Cos(other.Latitude * Math.PI / 180) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusMeters * c;
    }

    public void Validate()
    {
        if (Latitude is < 53.8 or > 54.0 || Longitude is < 27.4 or > 27.7)
        {
            throw new ArgumentException("Координаты должны быть в пределах Минска");
        }
    }
}
