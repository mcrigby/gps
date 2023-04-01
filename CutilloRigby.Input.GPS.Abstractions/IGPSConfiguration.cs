namespace CutilloRigby.Input.GPS;

public interface IGPSConfiguration
{
    string PortName { get; set; }
    int BaudRate { get; set; }
    string Parity { get; set; }
    int DataBits { get; set; }
    string StopBits { get; set; }
}
