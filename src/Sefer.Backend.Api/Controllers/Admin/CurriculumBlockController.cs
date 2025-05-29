using Sefer.Backend.Api.Data.Models.Courses.Curricula;
using Sefer.Backend.Api.Data.Requests.Curricula;
using Sefer.Backend.Api.Models.Admin.Curriculum;
using Sefer.Backend.Api.Views.Admin.Course.Curricula;
using Sefer.Backend.Api.Views.Shared;

namespace Sefer.Backend.Api.Controllers.Admin;

public class CurriculumBlockController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/courses/curricula/blocks/{id:int}")]
    [ProducesResponseType(typeof(CurriculumBlockView), 200)]
    public async Task<IActionResult> GetCurriculaBlockById(int id)
    {
        var block = await GetCurriculumBlockView(id);
        if (block == null) return NotFound();
        return Json(block);
    }

    [HttpDelete("/courses/curricula/blocks/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> DeleteCurriculumBlock(int id)
    {
        var block = await Send(new GetCurriculumBlockByIdRequest(id));
        if (block == null) return NotFound();

        var revision = await Send(new GetCurriculumRevisionByIdRequest(block.CurriculumRevisionId));
        if (revision == null || revision.IsEditable == false) return StatusCode(400);

        var deleted = await Send(new DeleteCurriculumBlockRequest(block));
        return deleted == false ? StatusCode(400) : NoContent();
    }

    [HttpPost("/courses/curricula/blocks/name")]
    [ProducesResponseType(typeof(BooleanView), 200)]
    public async Task<IActionResult> IsNameUnique([FromBody] IsBlockNameUniquePostModel post)
    {
        if (post == null) return Json(new BooleanView { Response = true });
        var isUnique = await Send(new IsCurriculumBlockNameUniqueRequest(post.Id, post.CurriculumId, post.Year, post.Name));
        var view = new BooleanView(isUnique);
        return Json(view);
    }

    [HttpPost("/courses/curricula/blocks")]
    [ProducesResponseType(typeof(CurriculumBlockView), 201)]
    public async Task<ActionResult> InsertCurriculumBlock([FromBody] CurriculumBlockPostModel block)
    {
        var revision = await ValidateBlock(block, null);
        if (revision == null) return BadRequest();

        var blocks = await Send(new GetCurriculumBlocksByYearRequest(revision.Id, block.Year));
        var sequenceId = 0;
        if (blocks.Count != 0) sequenceId = blocks.Max(b => b.SequenceId) + 1;
        var model = block.ToModel(revision.Id, sequenceId);

        var added = await Send(new AddCurriculumBlockRequest(model));
        if (added == false) return BadRequest();

        await SaveBlockCourses(block, model.Id);

        var view = GetCurriculumBlockView(model.Id);
        return Json(view, 201);
    }

    [HttpPut("/courses/curricula/blocks/{blockId:int}")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> UpdateCurriculumBlock([FromBody] CurriculumBlockPostModel block, int blockId)
    {
        // Check if the block is an existing block
        var dbBlock = await Send(new GetCurriculumBlockByIdRequest(blockId));
        if (dbBlock == null) return NotFound();

        // check if the block is valid
        var revision = await ValidateBlock(block, blockId);
        if (revision == null) return BadRequest();

        // Update the fields of the block
        dbBlock.Description = block.Description;
        dbBlock.Name = block.Name;
        await Send(new UpdateCurriculumBlockRequest(dbBlock));

        // Update the course of block, by deletion and adding
        var courses = await Send(new GetCurriculumBlockCoursesRequest(dbBlock.Id));
        foreach (var course in courses)
        {
            await Send(new DeleteCurriculumBlockCourseRequest(course));
        }

        await SaveBlockCourses(block, blockId);

        // And return the result
        return StatusCode(202);
    }

    [HttpPost("/courses/curricula/blocks/sequence")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> UpdateBlockSequence([FromBody] CurriculumBlockSequencePostModel sequence)
    {
        if (sequence == null) return BadRequest();

        var curriculum = await Send(new GetCurriculumByIdRequest(sequence.CurriculumId));
        if (curriculum == null) return BadRequest();

        var revision = await Send(new GetEditingCurriculumRevisionRequest(curriculum.Id));
        var blocks = await Send(new GetCurriculumBlocksByYearRequest(revision.Id, sequence.Year));

        if (blocks.Count != sequence.Blocks.Count) return BadRequest();
        var lookup = blocks.ToDictionary(b => b.Id);

        if (sequence.Blocks.Any(blockId => lookup.ContainsKey(blockId) == false)) return BadRequest();

        for (var sequenceId = 0; sequenceId < blocks.Count; sequenceId++)
        {
            var blockId = sequence.Blocks[sequenceId];
            var block = lookup[blockId];
            block.SequenceId = sequenceId;
            await Send(new UpdateCurriculumBlockRequest(block));
        }

        return StatusCode(202);
    }

    private async Task<CurriculumRevision> ValidateBlock(CurriculumBlockPostModel block, int? blockId)
    {
        if (block == null || ModelState.IsValid == false) return null;
        var isUnique = await Send(new IsCurriculumBlockNameUniqueRequest(blockId, block.CurriculumId, block.Year, block.Name));
        if (!isUnique) return null;

        var curriculum = await Send(new GetCurriculumByIdRequest(block.CurriculumId));
        if (curriculum == null) return null;

        var revision = await Send(new GetEditingCurriculumRevisionRequest(curriculum.Id));
        if (revision == null || revision.IsEditable == false) return null;

        if (block.Courses == null || block.Courses.Count == 0) return null;
        foreach (var courseId in block.Courses)
        {
            var course = await Send(new GetCourseByIdRequest(courseId));
            if (course == null) return null;
        }

        return revision;
    }

    private async Task SaveBlockCourses(CurriculumBlockPostModel block, int blockId)
    {
        var courses = block.Courses
            .Select((c, index) => new CurriculumBlockCourse { BlockId = blockId, CourseId = c, SequenceId = index })
            .ToList();

        foreach (var course in courses)
        {
            await Send(new AddCurriculumBlockCourseRequest(course));
        }
    }

    private async Task<CurriculumBlockView> GetCurriculumBlockView(int blockId)
    {
        var block = await Send(new GetCurriculumBlockByIdRequest(blockId));
        if (block == null) return null;

        var courses = await Send(new GetCoursesByCurriculumBlockRequest(block.Id, false));
        var lookup = courses.ToDictionary(c => c.Id);

        var revision = await Send(new GetCurriculumRevisionByIdRequest(block.CurriculumRevisionId));
        if (revision == null) return null;

        var allCourses = await Send(new GetCoursesRequest());
        var available = allCourses.Where(course => !lookup.ContainsKey(course.Id)).ToList();
        return new CurriculumBlockView(block, revision.CurriculumId, courses, available);
    }
}