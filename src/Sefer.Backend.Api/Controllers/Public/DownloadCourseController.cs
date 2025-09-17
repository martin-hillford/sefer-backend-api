using Sefer.Backend.Api.Models.Public;
using Sefer.Backend.Api.Views.Public.Download;
using Course = Sefer.Backend.Api.Views.Public.Download.Course;
using Lesson = Sefer.Backend.Api.Views.Public.Download.Lesson;

namespace Sefer.Backend.Api.Controllers.Public;

public class DownloadCourseController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    /// <summary>
    /// Create one single JSON structure for a single course (current public revision).
    /// </summary>
    /// <param name="courseId">The course to download</param>
    /// <param name="includeMedia"></param>
    [HttpGet("/download-course/{courseId:int}")]
    public async Task<IActionResult> DownloadCourse(int courseId, [FromQuery] bool includeMedia = true)
    {
        var request = new DownloadRequest { CourseId = courseId,  IncludeMedia = includeMedia };
        return await DownloadCourse(request);
    }
    
    /// <summary>
    /// Create one single JSON structure for a single course revision
    ///  This structure will the source for the move to a separate course api that will use MongoDB
    /// </summary>
    /// <param name="courseRevisionId">The course revision to download</param>
    [Authorize(Roles = "Admin"), HttpGet("/download-course-revision/{courseRevisionId:int}")]
    public async Task<IActionResult> DownloadCourseRevision(int courseRevisionId)
    {
        var request = new DownloadRequest { CourseRevisionId = courseRevisionId };
        return await DownloadCourse(request);
    }
    
    /// <summary>
    /// Creates a full downloadable package of a course (published revision) or revision.
    /// Depending on the parameters, it will also include all images of the course.
    /// </summary>
    [HttpPost("/download-course")]
    public async Task<IActionResult> DownloadCourse(DownloadRequest request)
    {
        var dataRevision = await GetRevision(request);
        if (dataRevision == null) return BadRequest();
        
        // If the revision can't be edited anymore (either it is closed or published) checked if it is cached
        
        var dataCourse = await Send(new GetCourseByIdRequest(dataRevision.CourseId));
        var dictionary = await Send(new GetCourseDictionaryRequest(dataCourse.Id));
        var dataLessons = await Send(new GetLessonsByCourseRevisionRequest(dataRevision.Id));
        var course = new Course(dataCourse, dataRevision)
        {
            Dictionary = dictionary.Select(d => new CourseWord(d)).ToList()
        };

        foreach (var dataLesson in dataLessons)
        {
            var lesson = new Lesson(dataLesson);

            var openQuestions = await Send(new GetLessonOpenQuestionsRequest(dataLesson));
            var mediaElements = await Send(new GetLessonMediaElementsRequest(dataLesson));
            var textElements = await Send(new GetLessonTextElementsRequest(dataLesson));
            var boolQuestions = await Send(new GetLessonBoolQuestionsRequest(dataLesson));
            var multipleChoiceQuestions = await Send(new GetLessonMultipleChoiceQuestionsRequest(dataLesson));
            
            lesson.AddBlocks(openQuestions.Select(o => new Views.Public.Download.OpenQuestion(o)));
            lesson.AddBlocks(mediaElements.Select(m => new Views.Public.Download.MediaElement(m)));
            lesson.AddBlocks(textElements.Select(t => new TextElement(t)));
            lesson.AddBlocks(boolQuestions.Select(b => new Views.Public.Download.BoolQuestion(b)));
            lesson.AddBlocks(multipleChoiceQuestions.Select(o => new Views.Public.Download.MultipleChoiceQuestion(o)));
            
            course.Lessons.Add(lesson);
        }
        
        if(request.IncludeMedia) await IncludeMedia(course, request);
        return request.Compressed ? DownloadGzippedJson(course,$"course_{course.Id}.json.gz") : Json(course);
    }

    private async Task<CourseRevision> GetRevision(DownloadRequest request)
    {
        if (request.CourseRevisionId > 0)
        {
            return await Send(new GetCourseRevisionByIdRequest(request.CourseRevisionId));
        }
        return await Send(new GetPublishedCourseRevisionRequest(request.CourseId));
    }
    
    private async Task IncludeMedia(Course course, DownloadRequest request)
    {
        request.HttpClientFactory = ServiceProvider.GetService<IHttpClientFactory>();
        request.FileStorageService = ServiceProvider.GetService<IFileStorageService>();
        await course.IncludeMedia(request);
    }
}