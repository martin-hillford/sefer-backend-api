using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Sefer.Backend.Api.Shared.Cryptography;

public static class Hashing
{
    public static string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var bytes = KeyDerivation.Pbkdf2(password, saltBytes, KeyDerivationPrf.HMACSHA512, 10000, 16);
        return Convert.ToBase64String(bytes);
    }

    public static string Sha512(string value)
    {
        var data = Encoding.UTF8.GetBytes(value);
        return Sha512(data);
    }

    private static string Sha512(byte[] data)
    {
        var bytes = SHA512.HashData(data);
        return Convert.ToBase64String(bytes);
    }
}