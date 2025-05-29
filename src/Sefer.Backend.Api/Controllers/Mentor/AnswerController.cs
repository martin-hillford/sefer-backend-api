namespace Sefer.Backend.Api.Controllers.Mentor;

[Authorize(Roles = "Mentor")]
public class AnswerController(IServiceProvider serviceProvider) : UserController(serviceProvider)
{
    [HttpGet("/mentor/feedback/{enrollmentId:int}")]
    [ProducesResponseType(typeof(List<QuestionAnswer>), 200)]
    public async Task<ActionResult<List<QuestionAnswer>>> GetPreviousFeedback(int enrollmentId)
    {
        // Check the mentor
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();

        // Get all the feedback of the user
        var enrollment = await Mediator.Send(new GetEnrollmentByIdRequest(enrollmentId));
        var request = new GetPreviousFeedbackRequest(mentor.Id, enrollment.CourseRevisionId);
        return await Mediator.Send(request);
    }
}