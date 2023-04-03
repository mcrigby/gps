using CutilloRigby.Input.GPS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Harness;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseConsoleLifetime()
            .ConfigureHostOptions(options =>
            {
                options.ShutdownTimeout = TimeSpan.FromSeconds(30);
            })
            .ConfigureLogging(builder => 
                builder.AddConsole()
            )
            .ConfigureHostConfiguration(configurationBuilder => {
                configurationBuilder
                    .AddJsonFile("./appsettings.json");
            })
            .ConfigureServices((hostBuilder, services) =>
            {
                var gpsConfigurationSection = hostBuilder.Configuration.GetSection("GPS");
                var gpsConfigurationDictionary = gpsConfigurationSection
                    .Get<Dictionary<string, GPSConfiguration>>(options => options.ErrorOnUnknownConfiguration = true)
                    .ToDictionary(x => x.Key, x => (IGPSConfiguration)x.Value);

                services.AddGPS<Program>();
                services.AddGPSConfiguration(gpsConfigurationDictionary);
            })
            .Build();

        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopping.Register(async () => await host.StopAsync());

        await host.StartAsync(lifetime.ApplicationStopping);
        Run(host, lifetime.ApplicationStopped);
    }

    private static void Run(IHost? host, CancellationToken cancellationToken)
    {
        if (host == null)
            return;

        var gps = host.Services.GetRequiredService<IGPS<Program>>();
        gps.Start();
        var gpsStatus = host.Services.GetRequiredService<IGPSStatus>();

        while (!cancellationToken.IsCancellationRequested)
        {
            Thread.Sleep(1000);

            Console.Write($"{gpsStatus.DateTime.ToLocalTime():F} ");

            if (!gpsStatus.HasFix)
                Console.Write("NoFix ");

            Console.WriteLine($"{gpsStatus.LatitudeDMS()}, {gpsStatus.LongitudeDMS()}");
        }

        gps.Stop();
    }
}
