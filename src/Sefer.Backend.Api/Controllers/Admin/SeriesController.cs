using Sefer.Backend.Api.Data.Requests.CourseSeries;
using Sefer.Backend.Api.Models.Admin.Course;
using Sefer.Backend.Api.Views.Admin.Course;
using Sefer.Backend.Api.Views.Shared;
using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class SeriesController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/series")]
    [ProducesResponseType(typeof(List<SeriesView>), 200)]
    public async Task<IActionResult> GetSeries()
    {
        var series = await Send(new GetSeriesRequest());
        var view = series.Select(s => new SeriesView(s)).ToList();
        return Json(view);
    }

    [HttpGet("/series/{id:int}")]
    [ProducesResponseType(typeof(List<SeriesView>), 200)]
    public async Task<IActionResult> GetSeries(int id)
    {
        var series = await Send(new GetSeriesByIdRequest(id));
        if (series == null) return NotFound();
        var view = new SeriesView(series);
        return Json(view);
    }

    [HttpPost("/series/name")]
    [ProducesResponseType(typeof(BooleanView), 200)]
    public async Task<IActionResult> IsNameUnique([FromBody] IsNameUniquePostModel post)
    {
        if (post == null) return Json(new BooleanView { Response = true });
        var isUnique = await Send(new IsSeriesNameUniqueRequest(post.Id, post.Name));
        var view = new BooleanView(isUnique);
        return Json(view);
    }

    [HttpPost("/series")]
    [ProducesResponseType(typeof(SeriesView), 201)]
    public async Task<ActionResult> InsertSeries([FromBody] SeriesPostModel series)
    {
        if (series == null || !ModelState.IsValid) return BadRequest(ModelState.ValidationState);
        var model = series.ToModel();
        var added = await Send(new AddSeriesRequest(model));
        if (added == false) return BadRequest();
        var view = new SeriesView(model);
        return Json(view, 201);
    }

    [HttpPut("/series/{id:int}")]
    [ProducesResponseType(typeof(List<SeriesView>), 200)]
    public async Task<ActionResult> UpdateSeries(int id, [FromBody] SeriesPostModel series)
    {
        // Check if all conditions are met
        var model = await Send(new GetSeriesByIdRequest(id));
        if (model == null) return NotFound();
        if (series == null || !ModelState.IsValid) return BadRequest(ModelState.ValidationState);

        // now update field, no complex methods involved
        model.Description = series.Description;
        model.Level = series.Level;
        model.Name = series.Name;

        // Update end send the result
        var updated = await Send(new UpdateSeriesRequest(model));
        return updated ? StatusCode(202) : BadRequest();
    }

    [HttpPost("/series/{id:int}/publish")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> PublishSeries(int id)
    {
        var series = await Send(new GetSeriesByIdRequest(id));
        if (series == null) return NotFound();
        if (!await Send(new IsPublishableSeriesRequest(series.Id))) return StatusCode(412);
        var published = await Send(new PublishSeriesRequest(series));
        return published ? StatusCode(202) : StatusCode(400);
    }

    [HttpPost("/series/{id:int}/close")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> CloseSeries(int id)
    {
        var series = await Send(new GetSeriesByIdRequest(id));
        if (series == null) return NotFound();
        if (series.IsPublic == false) return StatusCode(412);

        series.IsPublic = false;
        var updated = await Send(new UpdateSeriesRequest(series));
        return updated ? StatusCode(202) : StatusCode(400);
    }

    [HttpDelete("/series/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> DeleteSeries(int id)
    {
        var series = await Send(new GetSeriesByIdRequest(id));
        if (series == null) return NotFound();
        var deleted = await Send(new DeleteSeriesRequest(series));
        return deleted ? NoContent() : StatusCode(400);
    }

    [HttpGet("/series/{id:int}/courses")]
    [ProducesResponseType(typeof(List<SeriesCoursesView>), 200)]
    public async Task<IActionResult> GetSeriesCourses(int id)
    {
        // Get the series
        var series = await Send(new GetSeriesByIdRequest(id));
        if (series == null) return NotFound();

        // Get the required and the available
        var included = await Send(new GetCoursesForSeriesRequest(series.Id));
        var notAvailable = included.Select(c => c.Id).ToHashSet();

        var courses = await Send(new GetCoursesRequest());
        var available = courses
            .Where(c => !notAvailable.Contains(c.Id))
            .OrderBy(c => c.Name);

        var view = new SeriesCoursesView(series, included, available);
        return Json(view);
    }

    [HttpPost("/series/{id:int}/courses")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> SaveSeriesCourses(int id, [FromBody] List<int> courseIds)
    {
        if (courseIds == null || courseIds.Count != courseIds.Distinct().Count()) return BadRequest();
        var series = await Send(new GetSeriesByIdRequest(id));
        if (series == null) return NotFound();

        var courses = new List<Course>();
        foreach (var courseId in courseIds)
        {
            var course = await Send(new GetCourseByIdRequest(courseId));
            if (course == null) return BadRequest();
            courses.Add(course);
        }

        var saved = await Send(new SetSeriesForCourseRequest(series.Id, courses));
        return saved ? StatusCode(202) : StatusCode(500);
    }

    [HttpPost("/series/sequence")]
    public async Task<ActionResult> SaveSeriesSequence([FromBody] List<int> series)
    {
        var saved = await Send(new UpdateSeriesSequenceRequest(series));
        return saved ? StatusCode(200) : StatusCode(400);
    }
}