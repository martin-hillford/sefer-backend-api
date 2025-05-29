// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Support;

/// <summary>
/// Class to contain the options to contact the pdf service
/// </summary>
public class PdfOptions
{
    /// <summary>
    /// The endpoint of the service itself (without the /generate)
    /// </summary>
    public string Service { get; set; }

    /// <summary>
    /// The ApiKey used to contact the PDF service
    /// </summary>
    public string ApiKey { get; set; }
}