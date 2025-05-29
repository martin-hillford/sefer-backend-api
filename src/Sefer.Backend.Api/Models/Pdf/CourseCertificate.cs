// This model is post to another 
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
using Sefer.Backend.Api.Services.Security.Checksums;

namespace Sefer.Backend.Api.Models.Pdf;

/// <summary>
/// The model that is used to generate a diploma
/// </summary>
/// <remarks>
/// Creates a new Course Certificate Data object
/// </remarks>
public class CourseCertificateModel(ISite site, IRegion region, Enrollment enrollment, string language)
{
    /// <summary>
    /// The enrollment that the user has completed
    /// </summary>
    public Enrollment Enrollment { get; set; } = enrollment;

    /// <summary>
    /// The site for which the certificate is made
    /// </summary>
    public ISite Site { get; set; } = site;

    /// <summary>
    /// The site for which the certificate is made
    /// </summary>
    public IRegion Region { get; set; } = region;

    /// <summary>
    /// The language of the
    /// </summary>
    public string Language { get; set; } = language;

    /// <summary>
    /// The base url for the images
    /// </summary>
    public string ContentUrl => $"{Site.StaticContentUrl}/{Region.Id}";

    /// <summary>
    /// The logo for the images
    /// </summary>
    public string Logo => Site.GetLogoLarge(Region);

    /// <summary>
    /// The name of the website
    /// </summary>
    public string WebsiteName => Site.Name;

    /// <summary>
    /// The name of the director of the website
    /// </summary>
    public string DirectorName => Region.Director;

    /// <summary>
    /// A checksum such that it can easily be checked that the user did not fake the certificate
    /// </summary>
    public string Checksum => EnrollmentChecksum.GetChecksum(Enrollment);

    /// <summary>
    /// The date the enrollment was completed
    /// </summary>
    public string ClosureDate => Enrollment.ClosureDate?.ToString("yyyy-MM-dd");
}