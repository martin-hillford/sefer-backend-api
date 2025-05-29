using System.IO;
using System.Text;

namespace Sefer.Backend.Api.Shared;

public static class FileUtils
{
    /// <summary>
    /// Returns a temporary directory
    /// </summary>
    /// <returns></returns>
    public static string GetTemporaryDirectory(string tempDirectoryName = null)
    {
        var tempName = tempDirectoryName ?? Path.GetRandomFileName();
        var tempDirectory = Path.Combine(Path.GetTempPath(), tempName);
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }

    public static MemoryStream ToStream(this string value) => ToStream(value, Encoding.UTF8);

    private static MemoryStream ToStream(this string value, Encoding encoding)
        => new (encoding.GetBytes(value ?? string.Empty));
}