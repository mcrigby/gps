using UnitsNet;

namespace CutilloRigby.Input.GPS;

public sealed class GPSStatus : IGPSStatus
{
    public GPSStatus()
    {
        HasFix = false;

        DateTime = DateTimeOffset.MinValue;

        Location = GeographicPosition.Zero;
        SpeedOverGround = Speed.Zero;
        Bearing = Angle.Zero;
    }

    public DateTimeOffset DateTime { get; set; }
    public bool HasFix { get; set; }
    public IGeographicPosition Location { get; set; }
    public Speed SpeedOverGround { get; set; }
    public Angle Bearing { get; set; }
}
