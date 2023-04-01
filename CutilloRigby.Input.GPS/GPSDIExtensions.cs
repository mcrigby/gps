using CutilloRigby.Input.GPS;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServoDIExtensions
{
    public static IServiceCollection AddGPS<T>(this IServiceCollection services)
    {
        services.AddSingleton<IGPS<T>, GPS<T>>();
        //services.TryAddSingleton<IServoChanged, ServoChanged>();

        return services;
    }

    public static IServiceCollection AddGPSConfiguration(this IServiceCollection services, 
        IDictionary<string, IGPSConfiguration>? source = null, Action<IGPSConfigurationFactory>? configure = null)
    {
        var gpsConfigurationFactory = new GPSConfigurationFactory(source ?? new Dictionary<string, IGPSConfiguration>());
        configure?.Invoke(gpsConfigurationFactory);

        services.AddSingleton<IGPSConfigurationFactory>(gpsConfigurationFactory);
        services.AddSingleton(typeof(IGPSConfiguration<>), typeof(GPSConfiguration<>));

        return services;
    }
}
