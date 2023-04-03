namespace CutilloRigby.Input.GPS;

public sealed class GPSChanged : IGPSChanged
{
    public event EventHandler<GPSChangedEventArgs> Changed = delegate { };

    public void Trigger(object sender, GPSChangedEventArgs eventArgs)
    {
        Changed(sender, eventArgs);
    }
}