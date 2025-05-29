namespace Sefer.Backend.Api.Services.Security.TwoAuth;

/// <summary>
/// This hasher class implements the actual crypto required
/// </summary>
internal static class Hasher
{
    /// <summary>
    /// Holds the number of digits used
    /// </summary>
    private const int Digits = 6;

    /// <summary>
    /// Creates a hash for the secret and the given counter
    /// </summary>
    internal static int Hash(string secret, long counter)
    {
        var key = Base32.Decode(secret);
        return Hash(key, counter);
    }

    private static int Hash(byte[] key, long counter)
    {
        var bytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

        var hmac = new HMACSHA1(key);
        var hash = hmac.ComputeHash(bytes);

        // Convert the 4 bytes into an integer, ignoring the sign.
        var offset = hash[^1] & 0xf;
        var binary =
            ((hash[offset] & 0x7f) << 24)
            | (hash[offset + 1] << 16)
            | (hash[offset + 2] << 8)
            | (hash[offset + 3]);

        var password = binary % (int)Math.Pow(10, Digits);
        return password;
    }
}