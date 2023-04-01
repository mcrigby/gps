using Microsoft.Extensions.Logging;

namespace CutilloRigby.Input.GPS;

public sealed class GPS<T> : IGPS<T>
{
    private readonly IGPS _gps;

    public GPS(IGPSConfiguration<T> gpsConfiguration, ILogger<GPS> logger)
    {
        _gps = new GPS(gpsConfiguration, logger);
    }

    public void Start() => _gps.Start();

    public void Stop() => _gps.Stop();
}