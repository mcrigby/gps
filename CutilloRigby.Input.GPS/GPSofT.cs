using Microsoft.Extensions.Logging;

namespace CutilloRigby.Input.GPS;

public sealed class GPS<T> : IGPS<T>
{
    private readonly IGPS _gps;

    public GPS(IGPSConfiguration<T> gpsConfiguration, IGPSStatus gpsStatus, ILogger<GPS> logger)
    {
        _gps = new GPS(gpsConfiguration, gpsStatus, logger);
    }

    public void Start() => _gps.Start();

    public void Stop() => _gps.Stop();
}