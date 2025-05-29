namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class CourseController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();

    [HttpGet("/student/courses/")]
    [ProducesResponseType(typeof(List<CourseSummaryView>), 200)]
    public async Task<IActionResult> GetCourses()
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        var courses = await Send(new GetPublishedCoursesRequest());
        var enrollments = await Send(new GetTakenCoursesOfStudentRequest(student.Id));
        var taken = enrollments.Select(e => e.CourseRevision.CourseId).ToHashSet();
        var available = courses.Where(c => taken.Contains(c.Id) == false);

        var studentCounts = await Send(new GetStudentsCountPerCourseRequest());
        var readingTime = await Send(new GetCourseReadingTimeRequest());
        var view = available.Select(c => new CourseSummaryView(c, readingTime[c.Id], _fileStorageService, studentCounts[c.Id]));
        return Json(view);
    }
}