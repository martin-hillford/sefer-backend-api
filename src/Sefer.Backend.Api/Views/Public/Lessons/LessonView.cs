using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Public.Lessons;

/// <summary>
/// The lesson model represent a lesson
/// </summary>
/// <inheritdoc cref="Sefer.Backend.Api.Data.JsonViews.LessonView"/>
/// Todo: Refactor
public class LessonView : Data.JsonViews.LessonView
{
    /// <summary>
    /// The content of lesson.
    /// </summary>
    public List<ContentBlockView> Content { get; init; }

    /// <summary>
    /// The course of the lesson
    /// </summary>
    public readonly CourseDisplayView Course;

    /// <summary>
    /// The course revision of the lesson
    /// </summary>
    public readonly CourseRevisionView CourseRevision;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <inheritdoc />
    public LessonView(Lesson model, IFileStorageService fileStorageService) : base(model)
    {
        CourseRevision = new CourseRevisionView(model.CourseRevision);
        Course = new CourseDisplayView(model.CourseRevision.Course, fileStorageService);
        Content = new List<ContentBlockView>();
        foreach (var content in model.Content)
        {
            switch (content)
            {
                case LessonTextElement element:
                    Content.Add(new ContentBlockView(element));
                    break;
                case MediaElement element:
                    Content.Add(new ContentBlockView(element));
                    break;
                case OpenQuestion question:
                    Content.Add(new ContentBlockView(question));
                    break;
                case BoolQuestion question:
                    Content.Add(new ContentBlockView(question));
                    break;
                case MultipleChoiceQuestion question:
                    Content.Add(new ContentBlockView(question));
                    break;
            }
        }
    }
}
