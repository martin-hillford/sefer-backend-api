// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Courses.Curricula;

/// <summary>
/// A curriculum contains courses organized in blocks. This is the class representing such a block. 
/// A block basically is a collection of course grouped with a name. (e.g. Q1 or Object-Oriented Programming)
/// </summary>
/// <inheritdoc cref="ModifyDateLogEntity"/>
public class CurriculumBlock : ModifyDateLogEntity
{
    #region Properties

    /// <summary>
    /// The curriculum revision this block belongs to (a comparable revision system is used as with the courses)
    /// </summary>
    public int CurriculumRevisionId { get; set; }

    /// <summary>
    /// The CurriculumBlock that is a predecessor of this CurriculumBlock 
    /// (Set when a previous CurriculumBlock was promoted)
    /// </summary>
    public int? PredecessorId { get; set; }

    /// <summary>
    /// The year in which this block is located (when smaller than 1 no years are assumed)
    /// </summary>
    public short Year { get; set; }

    /// <summary>
    /// Gets / Sets the sequence number for this CurriculumBlock. Does not have to be unique. 
    /// But it will be used for sorting the blocks within a curriculum
    /// </summary>
    [Required]
    public int SequenceId { get; set; }

    /// <summary>
    /// Gets / sets the name of the CurriculumBlock
    /// </summary>
    [Required, MaxLength(255), MinLength(3)]
    public string Name { get; set; }

    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string Description { get; set; }

    /// <summary>
    /// The curriculum revision this block belongs to (a comparable revision system is used as with the courses)
    /// </summary>
    [ForeignKey("CurriculumRevisionId")]
    public CurriculumRevision CurriculumRevision { get; set; }

    /// <summary>
    /// The CurriculumBlock that is a predecessor of this CurriculumBlock 
    /// (Set when a previous CurriculumBlock was promoted)
    /// </summary>
    [ForeignKey("PredecessorId")]
    public CurriculumBlock Predecessor { get; set; }

    /// <summary>
    /// Get or set a list with all the Course (using an object to wrap their order) with this Block
    /// </summary>
    public ICollection<CurriculumBlockCourse> BlockCourses { get; set; }

    #endregion

    #region Derived Properties

    /// <summary>
    /// Gets a list of all the courses for this block, ordered by the sequence id
    /// </summary>
    [NotMapped]
    public IEnumerable<Course> Courses => BlockCourses.OrderBy(b => b.SequenceId).Select(b => b.Course);

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new CurriculumBlock
    /// </summary>
    public CurriculumBlock()
    {
        BlockCourses = new List<CurriculumBlockCourse>();
    }

    #endregion

    #region Methods

    /// <summary>
    /// This method create a successor for this curriculum block given a new curriculum revision
    /// </summary>
    /// <param name="newRevision">The curriculum revision to create the block successor in</param>
    /// <returns>The created block</returns>
    public CurriculumBlock CreateSuccessor(CurriculumRevision newRevision)
    {
        return new CurriculumBlock
        {
            CurriculumRevisionId = newRevision.Id,
            Description = Description,
            Name = Name,
            SequenceId = SequenceId,
            Year = Year,
            PredecessorId = Id
        };
    }

    #endregion
}