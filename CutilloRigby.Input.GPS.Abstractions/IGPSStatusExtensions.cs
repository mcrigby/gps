namespace CutilloRigby.Input.GPS;

public static class IGPSStatusExtensions
{
    public static string LatitudeDMS(this IGPSStatus gpsStatus)
    {
        return gpsStatus.Location.LatitudeDMS();
    }
    public static string LatitudeDMS(this IGeographicPosition geographicPosition)
    {
        var indicator = geographicPosition.Latitude < 0 ? 'S' : 'N';
        var totalDegrees = Math.Abs(geographicPosition.Latitude);
        var degrees = Math.Floor(totalDegrees);
        var totalMinutes = (totalDegrees - degrees) * 60;
        var minutes = Math.Floor(totalMinutes);
        var seconds = (totalMinutes - minutes) * 60;

        return $"{degrees:n0}° {minutes:n0}' {seconds:n2}\" {indicator}";
    }
    public static string LongitudeDMS(this IGPSStatus gpsStatus)
    {
        return gpsStatus.Location.LongitudeDMS();
    }
    public static string LongitudeDMS(this IGeographicPosition geographicPostion)
    {
        var indicator = geographicPostion.Longitude < 0 ? 'W' : 'E';
        var totalDegrees = Math.Abs(geographicPostion.Longitude);
        var degrees = Math.Floor(totalDegrees);
        var totalMinutes = (totalDegrees - degrees) * 60;
        var minutes = Math.Floor(totalMinutes);
        var seconds = (totalMinutes - minutes) * 60;

        return $"{degrees:n0}° {minutes:n0}' {seconds:n2}\" {indicator}";
    }
}
