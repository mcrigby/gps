namespace CutilloRigby.Input.GPS;

public interface IGPSConfigurationFactory
{
    void AddGPSConfiguration(string name, IGPSConfiguration configuration);
    void AddGPSConfiguration<T>(IGPSConfiguration configuration);
    IGPSConfiguration GetGPSConfiguration(string name);
    IGPSConfiguration GetGPSConfiguration<T>();
}
