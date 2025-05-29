using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// The lesson model represent a lesson
/// </summary>
/// <inheritdoc cref="LessonView"/>
public class MentorLessonView : LessonView
{
    /// <summary>
    /// The content of lesson.
    /// </summary>
    public List<MentorContentBlockView> Content { get; init; }

    /// <summary>
    /// The course of the lesson
    /// </summary>
    public readonly CourseDisplayView Course;

    /// <summary>
    /// The course revision of the lesson
    /// </summary>
    public readonly CourseRevisionView CourseRevision;

    public MentorLessonView(Lesson model, IFileStorageService fileStorageService) : base(model)
    {
        CourseRevision = new CourseRevisionView(model.CourseRevision);
        Course = new CourseDisplayView(model.CourseRevision.Course, fileStorageService);
        Content = new List<MentorContentBlockView>();
        foreach (var content in model.Content)
        {
            switch (content)
            {
                case LessonTextElement element:
                    Content.Add(new MentorContentBlockView(element));
                    break;
                case MediaElement element:
                    Content.Add(new MentorContentBlockView(element));
                    break;
                case OpenQuestion question:
                    Content.Add(new MentorContentBlockView(question));
                    break;
                case BoolQuestion question:
                    Content.Add(new MentorContentBlockView(question));
                    break;
                case MultipleChoiceQuestion question:
                    Content.Add(new MentorContentBlockView(question));
                    break;
            }
        }
    }
}
