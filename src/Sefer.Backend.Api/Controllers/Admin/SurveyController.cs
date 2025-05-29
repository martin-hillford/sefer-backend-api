using Sefer.Backend.Api.Data.Requests.Surveys;
using Sefer.Backend.Api.Models.Admin.Course;
using Sefer.Backend.Api.Views.Admin.Content;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin,CourseMaker")]
public class SurveyController(IServiceProvider provider) : BaseController(provider)
{
    [HttpPut("/surveys/{id:int}")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> UpdateCourse([FromBody] SurveyPostModel post, int id)
    {
        if (post == null || ModelState.IsValid == false) return BadRequest();
        var survey = await Send(new GetSurveyByIdRequest(id));
        if (survey == null) return NotFound();

        var revision = await Send(new GetCourseRevisionByIdRequest(survey.CourseRevisionId));
        if (revision == null) return NotFound();
        if (revision.IsEditable == false) return BadRequest();

        survey.EnableCourseRating = post.EnableCourseRating && post.EnableSurvey;
        survey.EnableMentorRating = post.EnableMentorRating && post.EnableSurvey;
        survey.EnableSocialPermissions = post.EnableSocialPermissions && post.EnableSurvey;
        survey.EnableTestimonial = post.EnableTestimonial && post.EnableSurvey;
        survey.EnableSurvey = post.EnableSurvey;

        return await Send(new UpdateSurveyRequest(survey)) ? StatusCode(202) : BadRequest();
    }

    [HttpGet("/surveys/unprocessed-results")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetUnprocessedSurveyResults([FromQuery] int? limit)
    {
        var results = await Send(new GetUnprocessedSurveyResultsRequest(limit));
        var view = results.Select(r => new SurveyResultView(r));
        return Json(view);
    }

    [HttpGet("/surveys/results")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetSurveyResults([FromQuery] int? limit)
    {
        var results = await Send(new GetSurveyResultsRequest(limit));
        var view = results.Select(r => new SurveyResultView(r));
        return Json(view);
    }

    [HttpGet("/surveys/results/{resultId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetSurveyResultById(int resultId)
    {
        var result = await Send(new GetSurveyResultByIdExtensiveRequest(resultId));
        if (result == null) return NotFound();
        var view = new SurveyResultView(result);
        return Json(view);
    }

    [HttpPost("/surveys/results/{resultId}/processed")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SetSurveyResultAsProcessed(int resultId)
    {
        var result = await Send(new SetSurveyResultAsProcessedRequest(resultId));
        if (result != true) return NotFound();
        return NoContent();
    }
}