// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Course;

/// <summary>
/// The post model for a series. Please note, the IsPublic property is omitted,
/// since that will be handled in a separate publish function
/// </summary>
public class SeriesPostModel
{
    /// <summary>
    /// Gets / sets the name of the course
    /// </summary>
    [Required, MaxLength(255), MinLength(3)]
    public string Name { get; set; }

    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// The difficulty level of the course
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Levels Level { get; set; }

    /// <summary>
    /// Converts the posted series into a model course
    /// </summary>
    /// <returns>The series</returns>
    public Series ToModel()
    {
        return new Series
        {
            Description = Description,
            Level = Level,
            Name = Name,
            IsPublic = false
        };
    }
}
