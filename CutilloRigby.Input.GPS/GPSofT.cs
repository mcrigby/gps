using Microsoft.Extensions.Logging;

namespace CutilloRigby.Input.GPS;

public sealed class GPS<T> : IGPS<T>
{
    private readonly IGPS _gps;

    public GPS(IGPSConfiguration<T> gpsConfiguration, IGPSStatus gpsStatus, IGPSChanged gpsChanged, ILogger<GPS> logger)
    {
        _gps = new GPS(gpsConfiguration, gpsStatus, gpsChanged, logger);
    }

    public void Start() => _gps.Start();

    public void Stop() => _gps.Stop();
}