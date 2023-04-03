namespace CutilloRigby.Input.GPS;

public interface IGeographicPosition
{
    double Latitude { get; set; }
    double Longitude { get; set; }
    double EllipsoidalHeight { get; set; }
}
