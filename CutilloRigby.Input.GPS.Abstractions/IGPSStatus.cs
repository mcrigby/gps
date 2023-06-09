using UnitsNet;

namespace CutilloRigby.Input.GPS;

public interface IGPSStatus
{
    DateTimeOffset DateTime { get; set; }
    bool HasFix { get; set; }
    IGeographicPosition Location { get; set; }
    Speed SpeedOverGround { get; set; }
    Angle Bearing { get; set; }
}
