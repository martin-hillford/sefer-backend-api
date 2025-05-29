using QRCoder;

namespace Sefer.Backend.Api.Services.QRCode;

public static class Generator
{
    /// <summary>
    /// This method will generate a qr image for the given url
    /// </summary>
    /// <param name="text">The text that should be in the image</param>
    public static QRImage GetQrImage(string text)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.M);
        var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(4);
        var base64 = Convert.ToBase64String(qrCodeImage);
        return new QRImage(text, $"data:image/png;base64,{base64}");
    }
}