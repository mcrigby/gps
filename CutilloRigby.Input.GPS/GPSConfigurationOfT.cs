namespace CutilloRigby.Input.GPS;

public sealed class GPSConfiguration<T> : IGPSConfiguration<T>
{
    private readonly IGPSConfiguration gpsConfiguration;

    public GPSConfiguration(IGPSConfigurationFactory gpsConfigurationFactory)
    {
        gpsConfiguration = gpsConfigurationFactory.GetGPSConfiguration<T>();
    }
    
    public string PortName { get => gpsConfiguration.PortName; set => gpsConfiguration.PortName = value; }
    public int BaudRate { get => gpsConfiguration.BaudRate; set => gpsConfiguration.BaudRate = value; }
    public string Parity { get => gpsConfiguration.Parity; set => gpsConfiguration.Parity = value; }
    public int DataBits { get => gpsConfiguration.DataBits; set => gpsConfiguration.DataBits = value; }
    public string StopBits { get => gpsConfiguration.StopBits; set => gpsConfiguration.StopBits = value; }
}
