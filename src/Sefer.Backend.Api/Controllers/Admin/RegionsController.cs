namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class RegionsController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/sites")]
    [ProducesResponseType(typeof(IEnumerable<ISite>), 200)]
    public async Task<IActionResult> GetSites()
    {
        var sites = await Send(new GetSitesRequest());
        return Json(sites);
    }

    [HttpGet("/environment/sites")]
    [ProducesResponseType(typeof(IEnumerable<ISite>), 200)]
    public async Task<IActionResult> GetEnvironmentSites()
    {
        var environment = EnvVar.GetEnvironmentName();
        var sites = await Send(new GetSitesRequest());
        return Json(sites.Where(s => s.Environment == environment));
    }

    [HttpGet("/regions")]
    [ProducesResponseType(typeof(IEnumerable<IRegion>), 200)]
    public async Task<IActionResult> GetRegions()
    {
        var regions = await Send(new GetRegionRequest());
        return Json(regions);
    }

    [HttpGet("/users/mentors/{mentorId:int}/regions")]
    [ProducesResponseType(typeof(List<MentorWithCoursesView>), 200)]
    public async Task<IActionResult> GetMentorRegions(int mentorId)
    {
        var mentor = await Send(new GetUserByIdRequest(mentorId));
        if (mentor == null || mentor.IsMentor == false) return NotFound();

        var mentorRegions = await Send(new GetMentorRegionsRequest(mentor.Id));
        var lookup = mentorRegions.ToHashSet();

        var result = await Send(new GetRegionRequest());
        var regions = result.ToList();
        var available = regions.Where(r => !lookup.Contains(r.Id)).ToList();
        var selected = regions.Where(r => lookup.Contains(r.Id)).ToList();
        var view = new MentorWithRegionsView(mentor, selected, available);
        return Json(view);
    }

    [HttpPost("/users/mentors/{mentorId:int}/regions")]
    public async Task<ActionResult> SaveMentorCourses(int mentorId, [FromBody] List<string> submitted)
    {
        // check if the provided mentor can be found
        var mentor = await Send(new GetUserByIdRequest(mentorId));
        if (mentor == null || mentor.IsMentor == false) return NotFound();
        if (submitted == null) return BadRequest();

        // load all the information required
        var regions = await Send(new GetRegionRequest());
        var current = await Send(new GetMentorRegionsRequest(mentor.Id));

        // Validate incoming data
        var missing = submitted.Count(id => regions.All(region => region.Id != id));
        if (missing != 0) return BadRequest();

        // Determine which regions should be added and removed for the mentor
        var toRemove = current.Where(region => !submitted.Contains(region)).ToList();
        var toAdd = submitted.Where(id => current.All(region => region != id)).ToList();

        // And update the database
        await Send(new RemoveRegionsForMentorRequest(mentor.Id, toRemove));
        await Send(new AddRegionsForMentorRequest(mentor.Id, toAdd));

        // Done
        return StatusCode(204);
    }
}