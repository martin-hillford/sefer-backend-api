namespace Sefer.Backend.Api.Views.Mentor;

public class PersonalInvitationView(string qrCode, string shortUrl, string fullUrl)
{
    [JsonPropertyName("qrCode")]
    public readonly string QRCode = qrCode;

    public readonly string ShortUrl = shortUrl;

    public readonly string FullUrl = fullUrl;
}