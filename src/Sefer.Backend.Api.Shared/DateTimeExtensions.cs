using System.Globalization;

namespace Sefer.Backend.Api.Shared;

public static class DateTimeExtensions
{
    public static string ToPostgresTimestamp(this DateTime dateTime)
    {
        return dateTime.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fffffffzzz", CultureInfo.InvariantCulture);
    }
}