using System.Web;

namespace Sefer.Backend.Api.Services.Security.TwoAuth;

public class SetupGenerator
{
    /// <summary>
    /// Generates an object you will need so that the user can set up his Google Authenticator to be used with your app.
    /// </summary>
    /// <param name="issuer">Your app name or company for example.</param>
    /// <param name="accountIdentity">Name, Email or id of the user, without spaces, this will be shown in google authenticator.</param>
    /// <param name="accountSecretKey">A secret key which will be used to generate one time passwords. This key is the same needed for validating a passed TOTP.</param>
    /// <param name="qrCodeWidth">Height of the QR code. Default is 300px.</param>
    /// <param name="qrCodeHeight">Width of the QR code. Default is 300px.</param>
    /// <returns>TotpSetup with ManualSetupKey and QrCode.</returns>
    public Setup Generate(string issuer, string accountIdentity, string accountSecretKey, int qrCodeWidth = 200, int qrCodeHeight = 200)
    {
        // Ensure all input is urlencoded
        var encodedAccount = HttpUtility.UrlPathEncode(accountIdentity.Replace(" ", ""));
        var encodedSecretKey = HttpUtility.UrlPathEncode(accountSecretKey);
        var encodedIssuer = HttpUtility.UrlPathEncode(issuer);
        var provisionUrl = $"otpauth://totp/{encodedIssuer}:{encodedAccount}?secret={encodedSecretKey}&issuer={encodedIssuer}";

        var setup = new Setup
        {
            QrCodeImage = QRCode.Generator.GetQrImage(provisionUrl),
            ManualKey = encodedSecretKey
        };

        return setup;
    }

    /// <summary>
    /// This method will generate a secret key to use with the two-factor authentication
    /// </summary>
    /// <returns></returns>
    public static string GenerateSecretKey()
    {
        var random = RandomNumberGenerator.Create();
        var data = new byte[25];
        random.GetBytes(data);
        return Base32.Encode(data);
    }
}