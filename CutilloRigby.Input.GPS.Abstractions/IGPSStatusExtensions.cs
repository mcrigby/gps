using System.Globalization;

namespace CutilloRigby.Input.GPS;

public static class IGPSStatusExtensions
{
    private static readonly DateTimeFormatInfo formatProvider = CultureInfo.InvariantCulture.DateTimeFormat;

    public static DateTime DateTimeUTC(this IGPSStatus gpsStatus)
    {
        if (!DateTime.TryParseExact($"{gpsStatus.Date}{gpsStatus.TimeUTC}", "ddMMyyHHmmss.fff",
            formatProvider, DateTimeStyles.AssumeUniversal, out var result))
            return DateTime.MinValue;
        return result;
    }
    public static DateTime DateTimeUTCOld(this IGPSStatus gpsStatus)
    {
        if (gpsStatus.Date.Length != 6
            || !gpsStatus.Date.All(char.IsNumber)
            || gpsStatus.TimeUTC.Length != 6
            || !gpsStatus.TimeUTC.All(char.IsNumber))
            return DateTime.MinValue;
                
        try
        {
            var _day = int.Parse(gpsStatus.Date.Substring(0, 2));
            var _month = int.Parse(gpsStatus.Date.Substring(2, 2));
            var _year = 2000 + int.Parse(gpsStatus.Date.Substring(4, 2));

            var _hour = int.Parse(gpsStatus.TimeUTC.Substring(0, 2));
            var _minute = int.Parse(gpsStatus.TimeUTC.Substring(2, 2));
            var _second = int.Parse(gpsStatus.TimeUTC.Substring(4, 2));

            return new DateTime(_year, _month, _day, _hour, _minute, _second, 0);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
    public static double LatitudeDD(this IGPSStatus gpsStatus)
    {
        var result = double.Parse(gpsStatus.LatitudeDDM.Substring(0, 2))
            + (double.Parse(gpsStatus.LatitudeDDM.Substring(2)) / 60);

        // Put negative if South
        if (gpsStatus.NSIndicator == 'S')
            result *= -1;

        return result;
    }
    public static string LatitudeDMS(this IGPSStatus gpsStatus)
    {
        var degrees = double.Parse(gpsStatus.LatitudeDDM.Substring(0, 2));
        var minutes = double.Parse(gpsStatus.LatitudeDDM.Substring(2, 2));
        var seconds = double.Parse(gpsStatus.LatitudeDDM.Substring(4)) * 60;

        return $"{degrees:n0}° {minutes:n0}' {seconds:n2}\" {gpsStatus.NSIndicator}";
    }
    public static double LongitudeDD(this IGPSStatus gpsStatus)
    {
        var result = double.Parse(gpsStatus.LongitudeDDM.Substring(0, 3))
            + (double.Parse(gpsStatus.LongitudeDDM.Substring(3)) / 60);

        // Put negative if South
        if (gpsStatus.EWIndicator == 'W')
            result *= -1;

        return result;
    }
    public static string LongitudeDMS(this IGPSStatus gpsStatus)
    {
        var degrees = double.Parse(gpsStatus.LongitudeDDM.Substring(0, 3));
        var minutes = double.Parse(gpsStatus.LongitudeDDM.Substring(3, 2));
        var seconds = double.Parse(gpsStatus.LongitudeDDM.Substring(5)) * 60;

        return $"{degrees:n0}° {minutes:n0}' {seconds:n2}\" {gpsStatus.EWIndicator}";
    }
}
