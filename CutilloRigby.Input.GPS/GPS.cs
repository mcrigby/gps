using System.IO.Ports;
using System.Text;
using Iot.Device.Nmea0183;
using Iot.Device.Nmea0183.Sentences;
using Microsoft.Extensions.Logging;

namespace CutilloRigby.Input.GPS;

public sealed class GPS : IGPS
{
    private readonly SerialPort _serialPort;
    private readonly IGPSStatus _gpsStatus;

    private NmeaParser? _nmeaParser;
    public GPS(IGPSConfiguration gpsConfiguration, IGPSStatus gpsStatus, ILogger<GPS> logger)
    {
        if (gpsConfiguration == null)
            throw new ArgumentNullException(nameof (gpsConfiguration));

        if (!Enum.TryParse<Parity>(gpsConfiguration.Parity, out var parity))
            parity = Parity.None;
        if (!Enum.TryParse<StopBits>(gpsConfiguration.StopBits, out var stopBits))
            stopBits = StopBits.One;

        _gpsStatus = gpsStatus ?? throw new ArgumentNullException(nameof(gpsStatus));

        _receivedDataFragment = string.Empty;

        _serialPort = new SerialPort(
            portName: gpsConfiguration.PortName,
            baudRate: gpsConfiguration.BaudRate,
            parity: parity,
            dataBits: gpsConfiguration.DataBits,
            stopBits: stopBits);

        _serialPort.NewLine = "\r\n";
        //_serialPort.DataReceived += _serialPort_DataReceived;
        
        SetupLogging(logger);
    }

    private string _receivedDataFragment;

    private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs eventArgs)
    {
            byte[] _buffer;
            int _bytesRead;
            string _dataBuffer;
            string[] _line;
            
            DateTimeOffset lastMessageTime = DateTimeOffset.UtcNow;

            try
            {
                _buffer = new byte[_serialPort.BytesToRead];

                if (_buffer.Length == 0)
                    return;

                _bytesRead = _serialPort.Read(_buffer, 0, _buffer.Length);
                _dataBuffer = _receivedDataFragment + new string(UTF8Encoding.UTF8.GetChars(_buffer));

                _line = _dataBuffer.Split('\n'); // Split in to lines. LF is removed, CR remains.
                if (_line.Length == 0)
                    return;

                _receivedDataFragment = _line[_line.Length - 1]; // Last (incomplete) line is written back to buffer.

                for (int _l = 0; _l < _line.Length - 1; _l++)
                {
                    var sentence = TalkerSentence.FromSentenceString(_line[_l], out _);
                    if (sentence == null)
                        continue;
                    
                    var typed = sentence.TryGetTypedValue(ref lastMessageTime);
                    if (typed == null)
                    {
                        unknownSentence(sentence.Id);
                        continue;
                    }

                    if (!(typed is RecommendedMinimumNavigationInformation rmc))
                        continue;

                    ParseRMCData(rmc);

                    var fields = sentence.Fields.ToArray();
                }
            }
            catch (Exception ex)
            {
                parserError(ex);
                _receivedDataFragment = string.Empty;
            }
    }

    public void Start()
    {
        _serialPort.Open();

        _nmeaParser = new NmeaParser(_serialPort.PortName, _serialPort.BaseStream, _serialPort.BaseStream);
        _nmeaParser.OnNewSequence += (sink, sentence) => 
        {
            if (sentence is RecommendedMinimumNavigationInformation rmc)
                ParseRMCData(rmc);
        };
        _nmeaParser.OnParserError += (sink, message, error) => nmeaParserError(sink, message, error);
        _nmeaParser.OnNewTime += (d) => {
            _gpsStatus.DateTime = d;
        };
        _nmeaParser.OnNewPosition += (gp, t, s) => {
            _gpsStatus.Location = (GeographicPosition)gp;
            _gpsStatus.Bearing = t.HasValue ? t.Value : UnitsNet.Angle.Zero;
            _gpsStatus.SpeedKnots = s.HasValue ? s.Value : UnitsNet.Speed.Zero;
        };

        _nmeaParser.SendSentence(_nmeaParser, PMTKSentence.PMTK_API_SET_NMEA_OUTPUT_RMC);
        _nmeaParser.SendSentence(_nmeaParser, PMTKSentence.PMTK_SET_NMEA_UPDATERATE_1HZ);
        _nmeaParser.StartDecode();
    }

    public void Stop()
    {
        if (_nmeaParser != null)
        {
            _nmeaParser.SendSentence(_nmeaParser, PMTKSentence.PMTK_API_SET_NMEA_OUTPUT_ALL);
            _nmeaParser.SendSentence(_nmeaParser, PMTKSentence.PMTK_SET_NMEA_UPDATERATE_1HZ);
            _nmeaParser.StopDecode();
        }

        _serialPort.Close();
    }

    private void ParseRMCData(RecommendedMinimumNavigationInformation rmc)
    {
        // Get Date/Time of Fix
        _gpsStatus.DateTime = rmc.DateTime;

        // Is fix valid?
        _gpsStatus.HasFix = rmc.Status == NavigationStatus.Valid;

        if (!_gpsStatus.HasFix) //If No Fix, exit.
        {
            _gpsStatus.Location = GeographicPosition.Zero;
            _gpsStatus.SpeedKnots = UnitsNet.Speed.Zero;
            _gpsStatus.Bearing = UnitsNet.Angle.Zero;
            
            return;
        }

        // Location
        _gpsStatus.Location = (GeographicPosition)rmc.Position;

        // Speed (in Knots)
        _gpsStatus.SpeedKnots = rmc.SpeedOverGround;

        // Course (in degrees)
        _gpsStatus.Bearing = rmc.TrackMadeGoodInDegreesTrue;
    }

    private void SetupLogging(ILogger logger)
    {
        if (logger.IsEnabled(LogLevel.Warning))
        {
            unknownSentence = (sentenceId) =>
                logger.LogWarning("Unknown SentenceId {sentenceId}", sentenceId);
        }
        if (logger.IsEnabled(LogLevel.Error))
        {
            parserError = (ex) =>
                logger.LogError(ex, "Error Parsing Serial Data");
            nmeaParserError = (sink, message, error) => 
                logger.LogError("NMEA Parser Error occured {message}. {nmeaError}", message, error);
        }
    }

    private Action<Exception> parserError = (ex) => { };
    private Action<NmeaSinkAndSource, string, NmeaError> nmeaParserError = (sink, message, error) => { };
    private Action<SentenceId> unknownSentence = (sentenceId) => { };
}
