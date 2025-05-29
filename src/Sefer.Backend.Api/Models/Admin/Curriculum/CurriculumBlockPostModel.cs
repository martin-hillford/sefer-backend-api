using Sefer.Backend.Api.Data.Models.Courses.Curricula;
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Curriculum;

/// <summary>
/// This model is used to post a curriculum block
/// </summary>
public class CurriculumBlockPostModel
{
    /// <summary>
    /// The id this block belongs to
    /// </summary>
    public int CurriculumId { get; set; }

    /// <summary>
    /// The year in which this block is located (when smaller than 1 no years are assumed)
    /// </summary>
    public short Year { get; set; }

    /// <summary>
    /// Gets / sets the name of the CurriculumBlock
    /// </summary>
    [Required]
    [MaxLength(255)]
    [MinLength(3)]
    public string Name { get; set; }

    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// A list of courses that are set for this block (the sequence is implied)
    /// </summary>
    public List<int> Courses { get; set; }

    /// <summary>
    /// This method convert the input model to a database model
    /// </summary>
    /// <param name="curriculumRevisionId"></param>
    /// <param name="sequenceId"></param>
    /// <returns></returns>
    public CurriculumBlock ToModel(int curriculumRevisionId, int sequenceId)
    {
        return new CurriculumBlock
        {
            CurriculumRevisionId = curriculumRevisionId,
            Description = Description,
            Name = Name,
            SequenceId = sequenceId,
            Year = Year
        };
    }
}
