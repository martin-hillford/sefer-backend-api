using Sefer.Backend.Api.Views.Mentor;

namespace Sefer.Backend.Api.Controllers.Admin;

/// <summary>
/// This controller deals with all the course requests from an admin
/// </summary>
[Authorize(Roles = "Admin")]
public class MentorController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [HttpGet("/users/mentors/{mentorId:int}/students")]
    [ProducesResponseType(typeof(List<UserListView>), 200)]
    public async Task<IActionResult> GetMentorStudents(int mentorId)
    {
        // First get the mentor
        var mentor = await Send(new GetUserByIdRequest(mentorId));
        if (mentor == null || mentor.IsMentor == false) return NotFound();

        // Get a list of enrollments for the mentor
        var enrollments = await Send(new GetMentorActiveStudentsRequest(mentor.Id));
        var students = enrollments.Select(e => new MentorStudentView(e)).ToList();
        var view = new { Mentor = new UserListView(mentor), Students = students };
        return Json(view);
    }

    /// <summary>
    /// This method loads the mentor given the id and included the courses the mentor is giving
    /// </summary>
    /// <param name="mentorId">The id of the mentor</param>
    /// <response code="200">The mentor with it's courses </response>
    /// <response code="404">The mentor could not be found or the provided id is from a user which is not a mentor</response>
    [HttpGet("/users/mentors/{mentorId:int}/courses")]
    [ProducesResponseType(typeof(List<MentorWithCoursesView>), 200)]
    public async Task<IActionResult> GetMentorCourses(int mentorId)
    {
        var mentor = await Send(new GetUserByIdRequest(mentorId));
        if (mentor == null || mentor.IsMentor == false) return NotFound();

        var courses = await Send(new GetMentorCoursesRequest(mentor.Id));
        var lookup = courses.ToDictionary(c => c.Id);

        var allCourses = await Send(new GetCoursesRequest());
        var available = allCourses.Where(c => !lookup.ContainsKey(c.Id)).ToList();
        var view = new MentorWithCoursesView(mentor, courses, available);
        return Json(view);
    }

    [HttpPost("/users/mentors/{mentorId:int}/courses")]
    public async Task<ActionResult> SaveMentorCourses(int mentorId, [FromBody] MentorCoursePostModel post)
    {
        // check if the provided mentor can be found
        var mentor = await Send(new GetUserByIdRequest(mentorId));
        if (mentor == null || mentor.IsMentor == false) return NotFound();
        if (post == null) return BadRequest();

        // load all the information required
        var courses = await Send(new GetMentorCoursesRequest(mentor.Id));
        var currentCourses = courses.ToDictionary(c => c.Id);
        var allCourses = (await Send(new GetCoursesRequest())).ToDictionary(c => c.Id);
        var postedCourses = post.Courses.ToHashSet();

        // check if the post contains illegal courses
        foreach (var courseId in post.Courses)
        {
            if (allCourses.ContainsKey(courseId) == false) return BadRequest();
        }

        // Loop through the current courses and remove the ones not in the post
        foreach (var (courseId, course) in currentCourses)
        {
            if (postedCourses.Contains(courseId)) continue;
            await Send(new RemoveMentorForCourseRequest(course.Id, mentor.Id));
        }

        // Loop through the posted courses and add the ones not in the current
        foreach (var courseId in postedCourses)
        {
            if (currentCourses.ContainsKey(courseId)) continue;
            var course = allCourses[courseId];
            await Send(new SetMentorForCourseRequest(course.Id, mentor.Id));
        }

        // Done
        return StatusCode(204);
    }

    [HttpGet("/users/mentor-performance")]
    public async Task<ActionResult> GetMentorPerformance()
    {
        var mentors = await Send(new GetMentorsRequest());
        var performance = await Send(new GetMentorPerformanceRequest());
        var data = mentors.ToDictionary(
                mentor => mentor, 
                mentor => performance.TryGetValue(mentor.Id, out var value)
                    ? value
                    : new MentorPerformance { MentorId = mentor.Id }
            );
        var view = data.Select(d => new PerformanceView(d.Key, d.Value)).OrderBy(m => m.Name).ToList();
        return Json(view);
    }
}