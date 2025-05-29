// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Services.Security.TwoAuth;

/// <summary>
/// Holds the setup for a user
/// </summary>
public class Setup
{
    /// <summary>
    /// The qr code for the user
    /// </summary>
    
    public QRImage QrCodeImage { get; set; }

    /// <summary>
    /// A key that user can type manually
    /// </summary>
    public string ManualKey { get; set; }
}