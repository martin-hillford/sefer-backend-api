// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Extensions;

/// <summary>
/// String Extensions
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Replaces multiple spaces from a string with single spaces
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string TrimSpaces(this string str)
    {
        return Regex.Replace(str, @"\s+", " ");
    }

    public static string FixBareLineFeeds(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
    }

    public static byte[] HexToBytes(this string hex)
    {
        // Ensure hex is in upper string
        hex = hex.ToUpper();

        // x variable used to hold byte array element position
        var byteIndex = 0;

        // allocate byte array based on half of string length
        var bytes = new byte[hex.Length / 2];

        // loop through the string - 2 bytes at a time converting it to decimal equivalent
        // and store in byte array
        while (hex.Length > (byteIndex * 2) + 1)
        {
            bytes[byteIndex] = Convert.ToByte(hex.Substring((byteIndex * 2), 2), 16);
            ++byteIndex;
        }
        // return the finished byte array of decimal values
        return bytes;
    }
}