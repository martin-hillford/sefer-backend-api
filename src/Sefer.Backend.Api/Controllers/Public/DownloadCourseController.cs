using Lesson = Sefer.Backend.Api.Models.Public.Download.Lesson;

namespace Sefer.Backend.Api.Controllers.Public;

public class DownloadCourseController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    /// <summary>
    /// Create one single json structure for a single course (current public revision). This structure will
    /// be the base for the downloadable package for offline mode. And will be the source for
    /// the move to a separate course api that will use MongoDB 
    /// </summary>
    /// <param name="courseId"></param>
    /// <param name="includeMedia"></param>
    /// <returns></returns>
    [HttpGet("/download-course/{courseId:int}")]
    public async Task<IActionResult> DownloadCourse(int courseId, [FromQuery] bool? includeMedia)
    {
        var dataRevision = await Send(new GetPublishedCourseRevisionRequest(courseId));
        if (dataRevision == null) return NotFound();
        
        var dataCourse = await Send(new GetCourseByIdRequest(courseId));
        var dataLessons = await Send(new GetLessonsByCourseRevisionRequest(dataRevision.Id));
        var course = new Models.Public.Download.Course(dataCourse, dataRevision);

        foreach (var dataLesson in dataLessons)
        {
            var lesson = new Lesson(dataLesson);

            var openQuestions = await Send(new GetLessonOpenQuestionsRequest(dataLesson));
            var mediaElements = await Send(new GetLessonMediaElementsRequest(dataLesson));
            var textElements = await Send(new GetLessonTextElementsRequest(dataLesson));
            var boolQuestions = await Send(new GetLessonBoolQuestionsRequest(dataLesson));
            var multipleChoiceQuestions = await Send(new GetLessonMultipleChoiceQuestionsRequest(dataLesson));
            
            lesson.AddBlocks(openQuestions.Select(o => new Models.Public.Download.OpenQuestion(o)));
            lesson.AddBlocks(mediaElements.Select(m => new Models.Public.Download.MediaElement(m)));
            lesson.AddBlocks(textElements.Select(t => new Models.Public.Download.TextElement(t)));
            lesson.AddBlocks(boolQuestions.Select(b => new Models.Public.Download.BoolQuestion(b)));
            lesson.AddBlocks(multipleChoiceQuestions.Select(o => new Models.Public.Download.MultipleChoiceQuestion(o)));
            
            course.Lessons.Add(lesson);
        }
        
        return Json(course);
    }   
}