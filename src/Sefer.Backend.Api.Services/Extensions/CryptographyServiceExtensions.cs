using Sefer.Backend.Api.Services.Security.Abstractions;

namespace Sefer.Backend.Api.Services.Extensions;

public static class CryptographyServiceExtensions
{
    public static string GetUserActivationCode(this ICryptographyService service, User user)
    {
        var data = $"{user.Id}.{user.PasswordSalt}.{user.Email}.{user.Password}";
        return service.UrlHash(data)[..8].ToUpper();
    }

    public static bool IsAsciiAlphaNumeric(this byte value)
        => value is >= 48 and <= 57 or >= 65 and <= 90 or >= 97 and <= 122;

    public static string GetAlphaNumericString(this RandomNumberGenerator randomNumberGenerator, int length)
    {
        var builder = new StringBuilder();
        while (builder.Length != length)
        {
            builder.Append(randomNumberGenerator.GetAlphaNumericChar());
        }
        return builder.ToString();
    }

    private static char GetAlphaNumericChar(this RandomNumberGenerator randomNumberGenerator)
    {
        const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        byte number = 255;
        while (number >= chars.Length)
        {
            var buffer = new byte[1];
            randomNumberGenerator.GetBytes(buffer);
            number = buffer[0];
        }
        return chars[number];
    }

}

