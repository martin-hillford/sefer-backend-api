using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Admin.Lesson;

/// <summary>
/// The lesson model represents a lesson
/// </summary>
/// <inheritdoc cref="Sefer.Backend.Api.Data.JsonViews.LessonView"/>
public class LessonView : Data.JsonViews.LessonView
{
    /// <summary>
    /// The content of a lesson.
    /// </summary>
    public List<ContentBlockView> Content { get; init; }

    /// <summary>
    /// The course of the lesson
    /// </summary>
    public readonly CourseView Course;

    /// <summary>
    /// The course revision of the lesson
    /// </summary>
    public readonly CourseRevisionView CourseRevision;

    /// <summary>
    /// A protected query to view the preview
    /// </summary>
    public readonly string PreviewQuery;

    /// <summary>
    /// Creates a new View
    /// </summary>
    public LessonView(Data.Models.Courses.Lessons.Lesson model, string previewQuery) : base(model)
    {
        CourseRevision = new CourseRevisionView(model.CourseRevision);
        Course = new CourseView(model.CourseRevision.Course);
        Content = [];
        PreviewQuery = previewQuery;
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
