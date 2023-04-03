namespace CutilloRigby.Input.GPS;

public sealed class GeographicPosition : IGeographicPosition
{
    public GeographicPosition() { }
    public GeographicPosition(double latitude, double longitude, double ellipsoidalHeight)
    {
        Latitude = latitude;
        Longitude = longitude;
        EllipsoidalHeight = ellipsoidalHeight;
    }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double EllipsoidalHeight { get; set; }

    public static implicit operator GeographicPosition(Iot.Device.Common.GeographicPosition gp) => 
        new GeographicPosition(gp.Latitude, gp.Longitude, gp.EllipsoidalHeight);
    public static implicit operator Iot.Device.Common.GeographicPosition(GeographicPosition gp) => 
        new Iot.Device.Common.GeographicPosition(gp.Latitude, gp.Longitude, gp.EllipsoidalHeight);

    public static readonly IGeographicPosition Zero = new GeographicPosition();
}
