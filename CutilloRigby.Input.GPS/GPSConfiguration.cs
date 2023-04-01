namespace CutilloRigby.Input.GPS;

public sealed class GPSConfiguration : IGPSConfiguration
{
    public GPSConfiguration()
    {
        PortName = "/dev/ttyS0";
        BaudRate = 9600;
        Parity = "None";
        DataBits = 8;
        StopBits = "One";
    }

    public string PortName { get; set; }
    public int BaudRate { get; set; }
    public string Parity { get; set; }
    public int DataBits { get; set; }
    public string StopBits { get; set; }

    public static readonly IGPSConfiguration None = new GPSConfiguration() { PortName = string.Empty };
}
