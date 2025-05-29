// ReSharper disable UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.Models.Annotations;

namespace Sefer.Backend.Api.Models.Admin.Course;

/// <summary>
/// A curriculum object for posting and putting
/// </summary>
public class CurriculumPostModel
{
    /// <summary>
    /// Gets / sets the name of the curriculum
    /// </summary>
    [Required, MaxLength(255), MinLength(3)]
    public string Name { get; set; }

    /// <summary>
    /// Gets the permalink for the curriculum. This should be a unique entry.
    /// </summary>
    [RegularExpression(PermalinkFormatAttribute.Format)]
    public string Permalink { get; set; }

    /// <summary>
    /// Gets / sets the description for the curriculum.
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// The difficulty level of the curriculum
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Levels Level { get; set; }

    /// <summary>
    /// Converts the posted series into a model curriculum
    /// </summary>
    /// <returns>The curriculum</returns>
    public Data.Models.Courses.Curricula.Curriculum ToModel()
    {
        return new Data.Models.Courses.Curricula.Curriculum
        {
            Description = Description,
            Level = Level,
            Name = Name,
            Permalink = Permalink
        };
    }
}
