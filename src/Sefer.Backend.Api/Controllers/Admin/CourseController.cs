using Sefer.Backend.Api.Models.Admin.Course;
using Sefer.Backend.Api.Views.Admin.Course;
using Sefer.Backend.Api.Views.Shared;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin,CourseMaker")]
public class CourseController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();

    private const string HeaderImageName = "headerimage";

    private const string LargeImageName = "largeimage";

    private const string ThumbnailImageName = "thumbnailimage";

    private const string CourseImagePath = "course_covers";

    [HttpGet("/courses")]
    public async Task<ActionResult<List<CourseView>>> GetCourses()
    {
        var courses = await Send(new GetCoursesWithRevisionRequest());
        var view = courses.Select(c => new CourseView(c, _fileStorageService)).ToList();
        return Json(view);
    }

    [HttpPost("/courses")]
    [ProducesResponseType(typeof(CourseView), 201)]
    public async Task<ActionResult<CourseView>> InsertCourse([FromBody] CoursePostModel course)
    {
        if (course == null || ModelState.IsValid == false) return BadRequest(ModelState.ValidationState);
        var model = course.ToModel();
        var added = await Send(new AddCourseRequest(model));
        if (added == false) return BadRequest();
        var view = new CourseView(model, _fileStorageService);
        return Json(view, 201);
    }

    [HttpGet("/courses/{id:int}")]
    [ProducesResponseType(typeof(CourseEditView), 200)]
    public async Task<IActionResult> GetCourse(int id)
    {
        var course = await Send(new GetCourseByIdRequest(id, true));
        if (course == null) return NotFound();
        var view = new CourseEditView(course, _fileStorageService);
        return Json(view);
    }

    [HttpGet("/courses/{id:int}/mentors")]
    [ProducesResponseType(typeof(CourseEditView), 200)]
    public async Task<IActionResult> GetCourseMentors(int id)
    {
        var course = await Send(new GetCourseByIdRequest(id, true));
        if (course == null) return NotFound();

        var mentors = await Send(new GetMentorsRequest());
        var assigned = await Send(new GetMentorsOfCourseRequest(course.Id));
        var available = mentors.Where(avm => assigned.Any(asm => asm.Id == avm.Id) == false).ToList();

        var view = new CourseMentorView(course, assigned, available);
        return Json(view);
    }

    [HttpPut("/courses/{id:int}/mentors")]
    public async Task<ActionResult> UpdateMentorsOfCourse(int id, [FromBody] CourseMentorsPostModel data)
    {
        var course = await Send(new GetCourseByIdRequest(id, true));
        if (course == null) return NotFound();
        if (data?.Mentors == null) return BadRequest();

        var mentors = await Send(new GetMentorsRequest());
        var assigned = await Send(new GetMentorsOfCourseRequest(course.Id));

        var toRemove = assigned.Where(asm => data.Mentors.Contains(asm.Id) == false).ToList();
        var toAdd = mentors.Where(m => assigned.Any(asm => asm.Id == m.Id) == false && data.Mentors.Contains(m.Id)).ToList();

        await Task.WhenAll(toAdd.Select(mentor => Send(new SetMentorForCourseRequest(course.Id, mentor.Id))));
        await Task.WhenAll(toRemove.Select(mentor => Send(new RemoveMentorForCourseRequest(course.Id, mentor.Id))));

        return NoContent();
    }

    [HttpPut("/courses/{id:int}")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> UpdateCourse([FromBody] CoursePostModel course, int id)
    {
        if (course == null || ModelState.IsValid == false) return BadRequest();
        var model = await Send(new GetCourseByIdRequest(id, true));
        if (model == null) return NotFound();

        // update the model with the model with the course information
        if (model.IsEditable)
        {
            model.Name = course.Name;
            model.Permalink = course.Permalink;
        }

        model.Description = course.Description;
        model.Level = course.Level;
        model.IsVideoCourse = course.IsVideoCourse;
        model.Private = course.Private;
        model.ShowOnHomepage = course.ShowOnHomepage;
        model.Author = course.Author;
        model.MaxLessonSubmissionsPerDay = course.MaxLessonSubmissionsPerDay;
        model.WebshopLink = course.WebshopLink;
        model.Copyright = course.Copyright;
        model.Citation = course.Citation;
        model.IntroductionLink = course.IntroductionLink;
        model.CopyrightLogo = course.CopyrightLogo;

        var updated = await Send(new UpdateCourseRequest(model));
        return updated ? StatusCode(202) : BadRequest();
    }

    [ProducesResponseType(204)]
    [HttpDelete("/courses/{id:int}")]
    public async Task<ActionResult> DeleteCourse(int id)
    {
        var course = await Send(new GetCourseByIdRequest(id));
        if (course == null) return NotFound();
        if (course.IsEditable == false) return BadRequest();
        var deleted = await Send(new DeleteCourseRequest(course));
        return deleted ? StatusCode(204) : BadRequest();
    }

    [HttpGet("/courses/{id:int}/editing-revision")]
    [ProducesResponseType(typeof(CourseRevisionsView), 200)]
    public async Task<IActionResult> GetEditRevision(int id)
    {
        var course = await Send(new GetCourseByIdExtendedRequest(id));
        if (course == null) return NotFound();
        if (course.EditingCourseRevision == null || course.EditingCourseRevision.IsEditable == false) return StatusCode(500);
        var view = new CourseRevisionsView(_fileStorageService, course);
        return Json(view);
    }

    [HttpPost("/courses/permalink")]
    [ProducesResponseType(typeof(BooleanView), 200)]
    public async Task<ActionResult> IsPermalinkUnique([FromBody] IsPermalinkUniquePostModel post)
    {
        if (post?.Id == null) return Json(new BooleanView { Response = true });
        var course = await Send(new GetPublishedCourseByPermalinkRequest(post.Permalink));
        var view = new BooleanView(course == null || course.Id == post.Id);
        return Json(view);
    }

    [HttpPost("/courses/name")]
    [ProducesResponseType(typeof(BooleanView), 200)]
    public async Task<ActionResult> IsNameUnique([FromBody] IsNameUniquePostModel post)
    {
        if (post == null) return Json(new BooleanView { Response = true });
        var isUnique = await Send(new IsCourseNameUniqueRequest(post.Id, post.Name));
        var view = new BooleanView(isUnique);
        return Json(view);
    }

    [HttpPost("/courses/{id:int}/files/{name}")]
    [ProducesResponseType(201)]
    public async Task<ActionResult> UploadCourseImage(IFormFile file, string name, int id)
    {
        var course = await Send(new GetCourseByIdRequest(id, true));
        if (course == null) return BadRequest();
        IFile uploadedFile;

        switch (name.ToLower())
        {
            case HeaderImageName:
                await DeleteFileAsync(_fileStorageService, course.HeaderImage);
                uploadedFile = await UploadFileAsync(_fileStorageService, file, CourseImagePath, true);
                if (uploadedFile != null)
                {
                    course.HeaderImage = uploadedFile.Path;
                    await Send(new UpdateSingleCoursePropertyRequest(course, "HeaderImage"));
                }
                break;
            case LargeImageName:
                await DeleteFileAsync(_fileStorageService, course.LargeImage);
                uploadedFile = await UploadFileAsync(_fileStorageService, file, CourseImagePath, true);
                if (uploadedFile != null)
                {
                    course.LargeImage = uploadedFile.Path;
                    await Send(new UpdateSingleCoursePropertyRequest(course, "LargeImage"));
                }
                break;
            case ThumbnailImageName:
                await DeleteFileAsync(_fileStorageService, course.ThumbnailImage);
                uploadedFile = await UploadFileAsync(_fileStorageService, file, CourseImagePath, true);
                if (uploadedFile != null)
                {
                    course.ThumbnailImage = uploadedFile.Path;
                    await Send(new UpdateSingleCoursePropertyRequest(course, "ThumbnailImage"));
                }
                break;
            default:
                return BadRequest();

        }

        if (uploadedFile == null) return BadRequest();
        return Json(new FileView(uploadedFile), 201);
    }

    [HttpDelete("/courses/{id:int}/files/{name}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult> DeleteCourseImage(int id, string name)
    {
        var course = await Send(new GetCourseByIdRequest(id, true));
        if (course == null) return BadRequest();
        switch (name.ToLower())
        {
            case HeaderImageName:
                await DeleteFileAsync(_fileStorageService, course.HeaderImage);
                course.HeaderImage = string.Empty;
                await Send(new UpdateSingleCoursePropertyRequest(course, "HeaderImage"));
                break;
            case LargeImageName:
                await DeleteFileAsync(_fileStorageService, course.LargeImage);
                course.LargeImage = string.Empty;
                await Send(new UpdateSingleCoursePropertyRequest(course, "LargeImage"));
                break;
            case ThumbnailImageName:
                await DeleteFileAsync(_fileStorageService, course.ThumbnailImage);
                course.ThumbnailImage = string.Empty;
                await Send(new UpdateSingleCoursePropertyRequest(course, "ThumbnailImage"));
                break;
            default:
                return BadRequest();
        }
        return Ok();
    }
}