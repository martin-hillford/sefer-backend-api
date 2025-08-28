using Sefer.Backend.Api.Data.Requests.Curricula;
using Sefer.Backend.Api.Models.Admin.Course;
using Sefer.Backend.Api.Views.Admin.Course.Curricula;
using Sefer.Backend.Api.Views.Shared;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin,CourseMaker")]
public class CurriculumController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/courses/curricula")]
    [ProducesResponseType(typeof(List<CurriculumView>), 200)]
    public async Task<IActionResult> GetCurricula()
    {
        var curricula = await Send(new GetCurriculaRequest(true));
        var view = curricula.Select(c => new CurriculumView(c)).ToList();
        return Json(view);
    }

    [HttpGet("/courses/curricula/{id:int}")]
    [ProducesResponseType(typeof(CurriculumView), 200)]
    public async Task<IActionResult> GetCurriculaById(int id)
    {
        var curriculum = await Send(new GetCurriculumByIdRequest(id, true));
        if (curriculum == null) return NotFound();
        var view = new CurriculumView(curriculum);
        return Json(view);
    }

    [HttpGet("/courses/curricula/{id:int}/editing-revision")]
    [ProducesResponseType(typeof(CurriculumRevisionsView), 200)]
    public async Task<IActionResult> GetEditRevision(int id)
    {
        var curriculum = await Send(new GetCurriculumByIdRequest(id, true));
        if (curriculum == null) return NotFound();
        if (curriculum.EditingCurriculumRevision is not { IsEditable: true }) return StatusCode(500);
        curriculum.EditingCurriculumRevision.Blocks = await Send(new GetCurriculumBlocksRequest(curriculum.EditingCurriculumRevision.Id));
        var view = new CurriculumRevisionsView(curriculum);
        return Json(view);
    }

    [HttpPost("/courses/curricula/permalink")]
    [ProducesResponseType(typeof(BooleanView), 200)]
    public async Task<IActionResult> IsPermalinkUnique([FromBody] IsPermalinkUniquePostModel post)
    {
        if (post == null) return Json(new BooleanView { Response = true });
        var curriculum = await Send(new GetCurriculumByPermalinkRequest(post.Permalink));
        var view = new BooleanView(curriculum == null || curriculum.Id == post.Id);
        return Json(view);
    }

    [HttpPost("/courses/curricula/name")]
    [ProducesResponseType(typeof(BooleanView), 200)]
    public async Task<IActionResult> IsNameUnique([FromBody] IsNameUniquePostModel post)
    {
        if (post == null) return Json(new BooleanView { Response = true });
        var isUnique = await Send(new IsCurriculumNameUniqueRequest(post.Id, post.Name));
        var view = new BooleanView(isUnique);
        return Json(view);
    }

    [HttpPost("/courses/curricula")]
    [ProducesResponseType(typeof(Views.Shared.Courses.Curricula.CurriculumView), 201)]
    public async Task<ActionResult> InsertCurriculum([FromBody] CurriculumPostModel curriculum)
    {
        if (curriculum == null || !ModelState.IsValid) return BadRequest(ModelState.ValidationState);
        var model = curriculum.ToModel();
        var added = await Send(new AddCurriculumRequest(model));
        if (!added) return BadRequest();
        var view = new Views.Shared.Courses.Curricula.CurriculumView(model);
        return Json(view, 201);
    }

    [HttpPut("/courses/curricula/{id:int}")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> UpdateCurriculum([FromBody] CurriculumPostModel curriculum, int id)
    {
        if (curriculum == null || !ModelState.IsValid) return BadRequest();
        var model = await Send(new GetCurriculumByIdRequest(id, true));
        if (model == null) return NotFound();
        if (!model.IsEditable) return BadRequest();

        model.Level = curriculum.Level;
        model.Name = curriculum.Name;
        model.Description = curriculum.Description;
        model.Permalink = curriculum.Permalink;

        var updated = await Send(new UpdateCurriculumRequest(model));
        return updated ? StatusCode(202) : BadRequest();
    }

    [ProducesResponseType(204)]
    [HttpDelete("/courses/curricula/{id:int}")]
    public async Task<ActionResult> DeleteCurriculum(int id)
    {
        var curriculum = await Send(new GetCurriculumByIdRequest(id, true));
        if (curriculum == null) return NotFound();
        if (!curriculum.IsEditable) return BadRequest();
        var deleted = await Send(new DeleteCurriculumRequest(curriculum));
        return deleted ? NoContent() : BadRequest();
    }
}