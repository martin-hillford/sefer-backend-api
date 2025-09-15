namespace Sefer.Backend.Api.Support.Extensions;

public static class ObjectExtensions
{
    public static long ToUnixTimes(this DateTime dateTime)
    {
        // Ensure the DateTime is in UTC
        var utc = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();

        // Unix epoch start
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Total seconds between the two dates
        return (long)(utc - epoch).TotalSeconds;
    }
}