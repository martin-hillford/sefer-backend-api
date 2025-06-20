using Sefer.Backend.Api.Views.Admin.CoursePrerequisites;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin,CourseMaker")]
public class CoursePrerequisiteController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [HttpGet("/courses/{courseId:int}/prerequisites")]
    [ProducesResponseType(typeof(List<CourseView>), 200)]
    public async Task<IActionResult> GetCoursePrerequisites(int courseId)
    {
        // Get the course
        var course = await Send(new GetCourseByIdRequest(courseId));
        if (course == null) return NotFound();

        // Get the required and the available
        var required = await Send(new GetRequiredCoursesRequest(course.Id));
        var notAvailable = required.Select(c => c.Id).ToHashSet();
        notAvailable.Add(courseId);

        var courses = await Send(new GetCoursesRequest());
        var available = courses.Where(c => !notAvailable.Contains(c.Id)).OrderBy(c => c.Name);

        var view = new CourseView(course, required, available);
        return Json(view);
    }

    [HttpPut("/courses/{courseId:int}/prerequisites")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> SaveCoursePrerequisites(int courseId, [FromBody] List<int> post)
    {
        // Get the course
        var course = await Send(new GetCourseByIdRequest(courseId));
        if (course == null) return NotFound();

        // Create the hash set of post result
        var requiredPosted = post.Distinct().ToHashSet();
        var requiredCourses = await Send(new GetRequiredCoursesRequest(course.Id));
        var currentRequired = requiredCourses.Select(c => c.Id).ToHashSet();

        // loop through the posted required courses and add those that are new
        foreach (var posted in requiredPosted)
        {
            if (currentRequired.Contains(posted)) continue;
            var newReq = new CoursePrerequisite { CourseId = courseId, RequiredCourseId = posted };
            if (await Send(new AddCoursePrerequisiteRequest(newReq)) == false) return BadRequest();
        }

        // loop through the current required courses and delete those that are not posted
        foreach (var current in currentRequired)
        {
            if (requiredPosted.Contains(current)) continue;
            var delReq = new CoursePrerequisite { CourseId = courseId, RequiredCourseId = current };
            if (await Send(new DeleteCoursePrerequisiteRequest(delReq)) == false) return BadRequest();
        }

        // Done return
        return StatusCode(202);
    }
}