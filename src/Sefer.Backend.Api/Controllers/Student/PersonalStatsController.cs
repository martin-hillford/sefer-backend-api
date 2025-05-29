namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class PersonalStatsController(IServiceProvider serviceProvider) : UserController(serviceProvider)
{
    [HttpGet("/student/stats/tier")]
    public async Task<ActionResult<StudentTier>> GetStudentTier()
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return NotFound();
        
        var overall = await Mediator.Send(GetStudentTierRequest.Overall(student.Id));
        var year = await Mediator.Send(GetStudentTierRequest.ThisYear(student.Id));
        var month = await Mediator.Send(GetStudentTierRequest.ThisMonth(student.Id));
        var tiers = new { Month = month, Overall = overall, Year = year };

        return Json(tiers);
    }
}