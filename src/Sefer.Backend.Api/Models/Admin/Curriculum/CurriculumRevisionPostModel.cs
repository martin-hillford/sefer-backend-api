// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Curriculum;

/// <summary>
/// The model for posting changes in the curriculum revisions
/// </summary>
public class CurriculumRevisionPostModel
{
    /// <summary>
    /// Get the number of years in this curriculum
    /// </summary>
    /// <remarks>negative not allowed, 0 interpreted as years not used in the curriculum</remarks>
    [Range(0, byte.MaxValue)]
    public byte Years { get; set; }
}
