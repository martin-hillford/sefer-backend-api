namespace Sefer.Backend.Api.Services.Security.TwoAuth;

/// <summary>
/// This class helps with encoding and decoding keys
/// </summary>
public static class Base32
{
    /// <summary>
    /// Holds the allowed characters in a base32 string
    /// </summary>
    private const string AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    /// <summary>
    /// This method will create base32 of the given bytes
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Encode(byte[] data)
    {
        var output = new StringBuilder();
        for (var bitIndex = 0; bitIndex < data.Length * 8; bitIndex += 5)
        {
            var @byte = data[bitIndex / 8] << 8;
            if (bitIndex / 8 + 1 < data.Length)
                @byte |= data[bitIndex / 8 + 1];
            @byte = 0x1f & (@byte >> (16 - bitIndex % 8 - 5));
            output.Append(AllowedCharacters[@byte]);
        }

        return output.ToString();
    }

    /// <summary>
    /// This method will decode a base32 string into bytes
    /// </summary>
    /// <param name="base32"></param>
    /// <returns></returns>
    public static byte[] Decode(string base32)
    {
        base32 = base32.TrimEnd('=').ToUpper(); //remove padding characters
        int byteCount = base32.Length * 5 / 8; //this must be truncated
        byte[] returnArray = new byte[byteCount];

        byte curByte = 0, bitsRemaining = 8;
        int arrayIndex = 0;

        foreach (var cValue in base32.Select(c => AllowedCharacters.IndexOf(c)))
        {
            int mask;
            if (bitsRemaining > 5)
            {
                mask = cValue << (bitsRemaining - 5);
                curByte = (byte)(curByte | mask);
                bitsRemaining -= 5;
            }
            else
            {
                mask = cValue >> (5 - bitsRemaining);
                curByte = (byte)(curByte | mask);
                returnArray[arrayIndex++] = curByte;
                curByte = (byte)(cValue << (3 + bitsRemaining));
                bitsRemaining += 3;
            }
        }

        //if we didn't end with a full byte ( the padding we have removed)
        if (arrayIndex != byteCount) returnArray[arrayIndex] = curByte;

        return returnArray;
    }
}