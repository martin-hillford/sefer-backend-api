namespace Sefer.Backend.Api.Services.QRCode;

/// <summary>
/// Holds a qr image
/// </summary>
/// <remarks>
/// Creates a new qr image
/// </remarks>
/// <param name="url"></param>
/// <param name="image"></param>
public class QRImage(string url, string image)
{
    /// <summary>
    /// The url to use
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public string Url { get; private set; } = url;

    /// <summary>
    /// a base 64 string of the image
    /// </summary>
    public string Image { get; private set; } = image;
}
