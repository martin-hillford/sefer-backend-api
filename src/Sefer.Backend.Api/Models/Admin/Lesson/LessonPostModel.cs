// The lesson is a complex object posted as one object. As a result,
// some properties will not be set or updated in the code itself.
// ReSharper disable UnusedAutoPropertyAccessor.Global, CollectionNeverUpdated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Lesson;

/// <summary>
/// The lesson model represents a lesson posted for saving
/// </summary>
public class LessonPostModel
{
    /// <summary>
    /// Gets / sets the chapter / section number for this lesson
    /// </summary>
    [Required, MaxLength(50), MinLength(1)]
    public string Number { get; set; }

    /// <summary>
    ///  Gets / sets the name for this lesson
    /// </summary>
    [Required, MaxLength(50), MinLength(3)]
    public string Name { get; set; }

    /// <summary>
    /// Gets / sets what the student should read before the start of the lesson
    /// </summary>
    public string ReadBeforeStart { get; set; }

    /// <summary>
    /// Gets / sets a description for this lesson
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets / sets the id of the <see cref="Sefer.Backend.Api.Data.Models.Courses.CourseRevision"/>  this lesson belongs to
    /// </summary>
    public int CourseRevisionId { get; set; }

    /// <summary>
    /// Gets or sets the content of the lesson.
    /// While posting, there is no difference regarding the fields (to ease the posting)
    /// </summary>
    public List<ContentBlockPostModel> Content { get; set; }

    /// <summary>
    /// Creates a new lesson and ensures the content is initialized as an empty list
    /// </summary>
    public LessonPostModel()
    {
        Content = [];
    }

    /// <summary>
    /// This method converts the lesson to a data model object
    /// </summary>
    public Data.Models.Courses.Lessons.Lesson ToLesson()
    {
        return new Data.Models.Courses.Lessons.Lesson
        {
            CourseRevisionId = CourseRevisionId,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            Description = Description,
            Name = Name,
            Number = Number,
            ReadBeforeStart = ReadBeforeStart
        };
    }
}
