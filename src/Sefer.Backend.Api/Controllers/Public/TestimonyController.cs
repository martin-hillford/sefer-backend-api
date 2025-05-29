using Sefer.Backend.Api.Views.Public.Resources;

namespace Sefer.Backend.Api.Controllers.Public;

public class TestimonyController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [ResponseCache(Duration = 86400)]
    [HttpGet("/public/testimonies/overall")]
    public async Task<ActionResult<List<TestimonyView>>> GetOverallTestimonies([FromQuery] int? limit)
    {
        var testimonies = await Send(new GetOverallTestimoniesRequest(limit));
        var view = testimonies.Select(t => new TestimonyView(t)).ToList();
        return Json(view);
    }

    /// <summary>
    /// This method will load all the overall testimonies for the admin
    /// </summary>
    [ResponseCache(Duration = 14400)]
    [HttpGet("/public/testimonies/course/{courseId:int}")]
    public async Task<ActionResult<List<TestimonyView>>> GetTestimoniesByCourseId(int courseId)
    {
        var testimonies = await Send(new GetTestimoniesByCourseIdRequest(courseId));
        var view = testimonies.Select(t => new TestimonyView(t)).ToList();
        return Json(view);
    }
}