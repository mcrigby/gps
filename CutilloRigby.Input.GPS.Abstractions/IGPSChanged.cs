namespace CutilloRigby.Input.GPS;

public interface IGPSChanged
{
    void Trigger(object sender, GPSChangedEventArgs eventArgs);
    event EventHandler<GPSChangedEventArgs> Changed;
}