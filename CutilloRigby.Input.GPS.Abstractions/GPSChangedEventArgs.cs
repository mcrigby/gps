using UnitsNet;

namespace CutilloRigby.Input.GPS;

public sealed class GPSChangedEventArgs
{
    public IGeographicPosition? Location;
    public Angle Bearing;
    public Speed SpeedOverGround;
}