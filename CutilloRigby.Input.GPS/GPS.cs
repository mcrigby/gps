using System.IO.Ports;
using Microsoft.Extensions.Logging;

namespace CutilloRigby.Input.GPS;

public sealed class GPS : IGPS
{
    private readonly SerialPort _serialPort;

    public GPS(IGPSConfiguration gpsConfiguration, ILogger<GPS> logger)
    {
        if (!Enum.TryParse<Parity>(gpsConfiguration.Parity, out var parity))
            parity = Parity.None;
        if (!Enum.TryParse<StopBits>(gpsConfiguration.StopBits, out var stopBits))
            stopBits = StopBits.One;

        _serialPort = new SerialPort(
            portName: gpsConfiguration.PortName,
            baudRate: gpsConfiguration.BaudRate,
            parity: parity,
            dataBits: gpsConfiguration.DataBits,
            stopBits: stopBits);

        _serialPort.DataReceived += _serialPort_DataReceived;
        
        SetupLogging(logger);
    }

    private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs eventArgs)
    {

    }

    public void Start()
    {
        _serialPort.Open();
    }

    public void Stop()
    {
        _serialPort.Close();
    }

    private void SetupLogging(ILogger logger)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {

        }
    }
}
