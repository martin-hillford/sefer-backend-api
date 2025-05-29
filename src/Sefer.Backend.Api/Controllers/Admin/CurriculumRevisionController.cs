using Sefer.Backend.Api.Data.Requests.Curricula;
using Sefer.Backend.Api.Models.Admin.Curriculum;
using Sefer.Backend.Api.Views.Shared;

namespace Sefer.Backend.Api.Controllers.Admin;

public class CurriculumRevisionController(IServiceProvider provider) : BaseController(provider)
{
    [HttpPut("/courses/curricula/revision/{id:int}")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> UpdateEditingRevision([FromBody] CurriculumRevisionPostModel model, int id)
    {
        if (model == null) return BadRequest();
        var revision = await Send(new GetCurriculumRevisionByIdRequest(id));

        if (revision == null) return NotFound();
        if (revision.IsEditable == false) return BadRequest();

        revision.Years = model.Years;
        var updated = await Send(new UpdateCurriculumRevisionRequest(revision));
        if (updated == false) return BadRequest();

        var blocks = await Send(new GetCurriculumBlocksRequest(revision.Id));
        foreach (var block in blocks)
        {
            if (block.Year == 0 && revision.Years != 0)
            {
                block.Year = 1;
                await Send(new UpdateCurriculumBlockRequest(block));
            }
            else if (block.Year > revision.Years)
            {
                block.Year = revision.Years;
                await Send(new UpdateCurriculumBlockRequest(block));
            }
        }

        return StatusCode(202);
    }

    [HttpGet("/courses/curricula/revision/{id:int}/publishable")]
    [ProducesResponseType(typeof(List<BooleanView>), 200)]
    public async Task<IActionResult> IsPublishable(int id)
    {
        var revision = await Send(new GetCurriculumRevisionByIdRequest(id));
        if (revision == null) return NotFound();
        var isPublishable = await Send(new IsCurriculumRevisionPublishableRequest(revision.Id));
        return Json(new BooleanView(isPublishable));
    }

    [HttpPost("/courses/curricula/revision/{id:int}/publish")]
    [ProducesResponseType(typeof(List<BooleanView>), 200)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Publish(int id)
    {
        var revision = await Send(new GetCurriculumRevisionByIdRequest(id));
        if (revision == null) return NotFound();
        var isPublishable = await Send(new IsCurriculumRevisionPublishableRequest(revision.Id));
        if (isPublishable == false) return StatusCode(412);
        var published = await Send(new PublishCurriculumRevisionRequest(revision.Id));
        return published ? StatusCode(202) : StatusCode(400);
    }

    [HttpPost("/courses/curricula/revision/{id:int}/close")]
    [ProducesResponseType(202)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Close(int id)
    {
        var revision = await Send(new GetCurriculumRevisionByIdRequest(id));
        if (revision == null) return NotFound();
        if (revision.Stage != Stages.Published) return StatusCode(412);
        var closed = await Send(new CloseCurriculumRevisionRequest(revision.Id));
        return closed ? StatusCode(202) : StatusCode(400);
    }
}