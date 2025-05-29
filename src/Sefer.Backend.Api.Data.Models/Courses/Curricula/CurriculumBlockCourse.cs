// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Courses.Curricula;

/// <summary>
/// Join for a course that is in a block of a curriculum
/// </summary>
/// <inheritdoc cref="Entity"/>
public class CurriculumBlockCourse : Entity
{
    /// <summary>
    /// Gets / Sets the sequence number for this CurriculumBlockCourse. Does not have to be unique. 
    /// But it will be used for sorting the blocks within a curriculum
    /// </summary>
    [Required]
    public int SequenceId { get; set; }

    /// <summary>
    /// Gets / sets the course for this block
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets / sets the block to which this course(block) belongs
    /// </summary>
    [Required]
    public int BlockId { get; set; }

    /// <summary>
    /// Gets / sets the course for this block
    /// </summary>
    [ForeignKey("CourseId")]
    public Course Course { get; set; }

    /// <summary>
    /// Gets / sets the block to which this course(block) belongs
    /// </summary>
    [ForeignKey("BlockId")]
    public CurriculumBlock Block { get; set; }
}