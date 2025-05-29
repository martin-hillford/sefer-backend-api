// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Courses.Lessons;

public class LessonContentState
{
    public int LessonId { get; set; }

    public ContentState ContentState { get; set; }
}