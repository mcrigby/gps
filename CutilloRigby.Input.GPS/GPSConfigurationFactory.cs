namespace CutilloRigby.Input.GPS;

public sealed class GPSConfigurationFactory : IGPSConfigurationFactory
{
    private readonly IDictionary<string, IGPSConfiguration> _source;

    public GPSConfigurationFactory(IDictionary<string, IGPSConfiguration> source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public void AddGPSConfiguration(string name, IGPSConfiguration configuration)
    {
        if (!_source.ContainsKey(name))
            _source.Add(name, configuration);
        else
            _source[name] = configuration;
    }

    public void AddGPSConfiguration<T>(IGPSConfiguration configuration)
    {
        AddGPSConfiguration(typeof(T).FactoryName(), configuration);
    }

    public IGPSConfiguration GetGPSConfiguration(string name)
    {
        if (!_source.ContainsKey(name))
            return GPSConfiguration.None;
        else
            return _source[name];
    }

    public IGPSConfiguration GetGPSConfiguration<T>()
    {
        return GetGPSConfiguration(typeof(T).FactoryName());
    }
}
