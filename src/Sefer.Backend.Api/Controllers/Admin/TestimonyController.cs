using Sefer.Backend.Api.Models.Admin.Resources;
using Sefer.Backend.Api.Views.Admin.Resources;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class TestimonyController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/testimonies")]
    public async Task<ActionResult<List<TestimonyView>>> GetTestimonies()
    {
        var testimonies = await Send(new GetTestimoniesRequest());
        var view = testimonies.Select(t => new TestimonyView(t));
        return Json(view);
    }

    [HttpGet("/testimonies/{id:int}")]
    public async Task<ActionResult<TestimonyView>> GetTestimony(int id)
    {
        var testimony = await Send(new GetTestimonyByIdRequest(id));
        if (testimony == null) return NotFound();
        var view = new TestimonyView(testimony);
        return Json(view);
    }

    [HttpGet("/testimonies/course/{courseId:int}")]
    public async Task<ActionResult<List<TestimonyView>>> GetTestimoniesByCourseId(int courseId)
    {
        var testimonies = await Send(new GetTestimoniesByCourseIdRequest(courseId));
        var view = testimonies.Select(t => new TestimonyView(t)).ToList();
        return Json(view);
    }

    [HttpGet("/testimonies/overall")]
    public async Task<ActionResult<List<TestimonyView>>> GetOverallTestimonies([FromQuery] int? limit)
    {
        var testimonies = await Send(new GetOverallTestimoniesRequest(limit));
        var view = testimonies.Select(t => new TestimonyView(t)).ToList();
        return Json(view);
    }

    [HttpDelete("/testimonies/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> DeleteTestimony(int id)
    {
        var testimony = await Send(new GetTestimonyByIdRequest(id));
        if (testimony == null) return NotFound();
        var deleted = await Send(new DeleteTestimonyRequest(testimony));
        return deleted ? NoContent() : StatusCode(500);
    }

    [HttpPost("/testimonies")]
    public async Task<ActionResult> AddTestimony([FromBody] TestimonyPostModel testimony)
    {
        if (ModelState.IsValid == false) return BadRequest();
        var model = testimony.ToDataModel();
        var added = await Send(new AddTestimonyRequest(model));
        var view = new TestimonyView(model);
        if (added == false) return BadRequest();
        return Json(view, 201);
    }

    [HttpPut("/testimonies/{id:int}")]
    public async Task<ActionResult> UpdateTestimony(int id, [FromBody] Testimony testimony)
    {
        // Check if all conditions are met
        var model = await Send(new GetTestimonyByIdRequest(id));
        if (model == null) return NotFound();
        if (testimony == null || ModelState.IsValid == false) return BadRequest(ModelState.ValidationState);

        // now update fields, no complex methods involved
        model.Content = testimony.Content;
        model.CourseId = testimony.CourseId;
        model.StudentId = testimony.StudentId;
        model.Name = testimony.Name;
        model.IsAnonymous = testimony.IsAnonymous;

        // Update end send the result
        var updated = await Send(new UpdateTestimonyRequest(model));
        return updated ? StatusCode(202) : BadRequest();
    }
}