// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
using Sefer.Backend.Api.Data.Models.Courses.Curricula;
using Sefer.Backend.Api.Data.Models.Courses.Rewards;

namespace Sefer.Backend.Api.Models.Pdf;

/// <summary>
/// The model that is used to generate a diploma
/// </summary>
public class CurriculumDiploma
{
    /// <summary>
    /// The student that has the diploma
    /// </summary>
    public User Student { get; set; }

    /// <summary>
    /// The curriculum the student completed
    /// </summary>
    public Curriculum Curriculum { get; set; }

    /// <summary>
    /// The grant itself
    /// </summary>
    public RewardGrant Grant { get; set; }

    /// <summary>
    /// A list of enrollments that the user has completed
    /// </summary>
    public List<Enrollment> Enrollments { get; set; }

    /// <summary>
    /// The site for which the certificate is made
    /// </summary>
    public ISite Site { get; set; }

    /// <summary>
    /// The region for which the certificate is made
    /// </summary>
    public IRegion Region { get; set; }

    /// <summary>
    /// The language of the
    /// </summary>
    public string Language { get; set; }

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
    /// The date the curriculum was completed
    /// </summary>
    public string CompletionDate => Grant.Date.ToString("yyyy-MM-dd");
}