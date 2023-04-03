using Iot.Device.Nmea0183;
using Iot.Device.Nmea0183.Sentences;

namespace CutilloRigby.Input.GPS;

public static class PMTKSentence
{
    public static RawSentence SetNMEAUpdateRate(int updateRatems)
    {
        if (updateRatems < 100 || 1000 < updateRatems)
            throw new ArgumentOutOfRangeException(nameof(updateRatems), updateRatems, "Value must be between 100 and 1000.");
        
        var data = Enumerable.Range(0, 1)
            .Select(x => updateRatems.ToString())
            .ToArray();
        
        return new RawSentence(PM, TK220, data, DateTimeOffset.UtcNow);
    }
    public static RawSentence SetNMEAOutput(bool gll = false, bool rmc = false, bool vtg = false, bool gga = false, bool gsa = false, bool gsv = false)
    {
        var data = Enumerable.Range(0,19)
            .Select(x => false)
            .ToArray();
        
        data[0] = gll;
        data[1] = rmc;
        data[2] = vtg;
        data[3] = gga;
        data[4] = gsa;
        data[5] = gsv;

        return new RawSentence(PM, TK314, data.Select(x => x ? "1" : "0"), DateTimeOffset.UtcNow);
    }
    
    private static readonly TalkerId PM = new TalkerId('P', 'M');
    private static readonly SentenceId TK220 = new SentenceId("TK220");
    private static readonly SentenceId TK314 = new SentenceId("TK314");

    public static RawSentence PMTK_SET_NMEA_UPDATERATE_1HZ => SetNMEAUpdateRate(1000);
    public static RawSentence PMTK_SET_NMEA_UPDATERATE_5HZ => SetNMEAUpdateRate(200);
    public static RawSentence PMTK_SET_NMEA_UPDATERATE_10HZ => SetNMEAUpdateRate(100);

    public static RawSentence PMTK_API_SET_NMEA_OUTPUT_RMC => SetNMEAOutput(rmc: true);
    public static RawSentence PMTK_API_SET_NMEA_OUTPUT_RMCGGA => SetNMEAOutput(rmc: true, gga: true);
    public static RawSentence PMTK_API_SET_NMEA_OUTPUT_ALL => SetNMEAOutput(gll: true, rmc: true, vtg: true, gga: true, gsa: true, gsv: true);
    public static RawSentence PMTK_API_SET_NMEA_OUTPUT_NONE => SetNMEAOutput();
}