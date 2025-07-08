// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Data.Models.Courses;

/// <summary>
/// Represents a single word in the dictionary of the course
/// </summary>
public class CourseRevisionDictionaryWord
{
    public Guid Id { get; set; }
    
    public int CourseRevisionId { get; set; }
    
    [Required, MinLength(2), MaxLength(255)]
    public string Word { get; set; }
    
    [Required, MinLength(5), MaxLength(int.MaxValue)]
    public string Explanation { get; set; }
    
    [Required, MinLength(2), MaxLength(3)]
    public string Language { get; set; }
    
    [MaxLength(int.MaxValue)]
    public string PictureUrl { get; set; }
    
    [ForeignKey(nameof(CourseRevisionId))]
    public CourseRevision CourseRevision { get; set; }

    public CourseRevisionDictionaryWord CreateSuccessor(CourseRevision successorRevision)
    {
        return new CourseRevisionDictionaryWord
        {
            CourseRevisionId = successorRevision.Id,
            Word = Word,
            Explanation = Explanation,
            Language = Language,
            PictureUrl = PictureUrl
        };
    }
}