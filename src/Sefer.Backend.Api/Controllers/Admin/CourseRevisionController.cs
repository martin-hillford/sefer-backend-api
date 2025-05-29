using Sefer.Backend.Api.Data.Requests.Surveys;
using Sefer.Backend.Api.Models.Admin.CourseRevision;
using Sefer.Backend.Api.Views.Admin.Course;
using Sefer.Backend.Api.Views.Shared;
using Sefer.Backend.Api.Views.Shared.Courses.Lessons;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin,CourseMaker")]
public class CourseRevisionController(IServiceProvider provider) : BaseController(provider)
{
    [HttpPut("/courses/revision/{id:int}")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> UpdateEditingRevision([FromBody] CourseRevisionPostModel model, int id)
    {
        if (model == null) return BadRequest();
        var revision = await Send(new GetCourseRevisionByIdRequest(id));
        if (revision == null) return NotFound();
        if (revision.IsEditable == false) return BadRequest();
        revision.AllowSelfStudy = model.AllowSelfStudy;
        var updated = await Send(new UpdateCourseRevisionRequest(revision));
        return updated ? StatusCode(202) : StatusCode(500);
    }

    [HttpGet("/courses/revision/{id:int}/content-state")]
    public async Task<ActionResult> GetContentState(int id)
    {
        var request = new ContentStateRequest(id);
        var lessons = await Mediator.Send(request);
        if (lessons == null) return NotFound();

        return Ok(new { Lessons = lessons, CourseState = GetCourseState(lessons) });
    }

    [HttpDelete("/courses/revision/{id:int}")]
    public async Task<ActionResult> DeleteRevision(int id)
    {
        var request = new RemoveCourseRevisionRequest(id);
        var removed = await Mediator.Send(request);
        return removed ? Ok() : StatusCode(500);
    }

    [HttpGet("/courses/revision/{id:int}/publishable")]
    [ProducesResponseType(typeof(List<BooleanView>), 200)]
    public async Task<IActionResult> IsPublishable(int id)
    {
        var revision = await Send(new GetCourseRevisionByIdRequest(id));
        if (revision == null) return NotFound();
        var publishable = await Send(new IsPublishableCourseRevisionRequest(revision.Id));
        return Json(new BooleanView(publishable));
    }

    [HttpGet("/courses/revision/{id:int}/survey")]
    [ProducesResponseType(typeof(SurveyView), 200)]
    public async Task<IActionResult> GetSurvey(int id)
    {
        var revision = await Send(new GetCourseRevisionByIdRequest(id));
        if (revision == null) return NotFound();

        var course = await Send(new GetCourseByIdRequest(revision.CourseId));
        if (course == null) return StatusCode(500);

        var survey = await Send(new GetSurveyByCourseRevisionRequest(revision.Id));
        if (survey == null) return StatusCode(500);

        return Json(new SurveyView(survey, revision, course));
    }

    [HttpPost("/courses/revision/{id:int}/publish")]
    [ProducesResponseType(typeof(List<BooleanView>), 200)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Publish(int id)
    {
        var revision = await Send(new GetCourseRevisionByIdRequest(id));
        if (revision == null) return NotFound();

        var publishable = await Send(new IsPublishableCourseRevisionRequest(revision.Id));
        if (!publishable) return StatusCode(412);

        var published = await Send(new PublishCourseRevisionRequest(revision.Id));
        return published ? StatusCode(202) : StatusCode(400);
    }

    [HttpPost("/courses/revision/{id:int}/close")]
    [ProducesResponseType(202)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Close(int id)
    {
        var revision = await Send(new GetCourseRevisionByIdRequest(id));
        if (revision == null) return NotFound();
        if (revision.Stage != Stages.Published) return StatusCode(412);
        var closed = await Send(new CloseCourseRevisionRequest(revision.Id));
        return closed ? StatusCode(202) : StatusCode(400);
    }

    [HttpPost("/courses/revision/{id:int}/lesson-sequence")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> UpdateLessonSequence([FromBody] List<int> lessons, int id)
    {
        // check for correct posted revision
        var revision = await Send(new GetCourseRevisionByIdRequest(id));
        if (revision == null) return NotFound();
        if (revision.IsEditable == false) return BadRequest();

        // Check for correct loaded lessons
        if (lessons == null) return BadRequest();
        lessons = lessons.Distinct().ToList();
        var dbLessons = await Send(new GetLessonsByCourseRevisionRequest(revision.Id));
        if (dbLessons.Count != lessons.Count) return BadRequest();

        // create lookup dictionary
        var lookup = new Dictionary<int, int>();
        for (var i = 0; i < lessons.Count; i++)
        {
            lookup.Add(lessons[i], i);
        }

        // check if all the lesson are provided and set the sequence id
        foreach (var lesson in dbLessons)
        {
            if (lookup.TryGetValue(lesson.Id, out var value) == false) return BadRequest();
            lesson.SequenceId = value;
        }

        // update all the lessons
        // Todo: deal with errors while updating
        foreach (var lesson in dbLessons)
        {
            await Send(new UpdateLessonRequest(lesson));
        }

        return StatusCode(202);
    }

    [HttpGet("/courses/revision/{id:int}/questions")]
    [ProducesResponseType(200)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CourseRevisionCheckQuestionView>> GetQuestionForCourseRevision(int id)
    {
        // Load the course revision
        var revision = await Send(new GetCourseRevisionByIdRequest(id));
        if (revision == null) return NotFound();

        // load all the question and also load the course
        var allQuestions = await Send(new GetQuestionsOfCourseRevisionRequest(revision.Id));
        var course = await Send(new GetCourseByIdRequest(revision.CourseId));
        if (course == null) return NotFound();

        // Create the views
        var lessons = allQuestions.Select(t => new LessonCheckQuestionView(t.Lesson, t.Questions)).ToList();
        var view = new CourseRevisionCheckQuestionView(course, lessons, revision);

        // The whole loading is done, return the result
        return Json(view);
    }

    private static ContentState GetCourseState(IReadOnlyCollection<LessonContentState> lessons)
    {
        if (lessons.All(l => l.ContentState == ContentState.Html)) return ContentState.Html;
        return lessons.All(l => l.ContentState == ContentState.MarkDown) ? ContentState.MarkDown : ContentState.Mixed;
    }
}