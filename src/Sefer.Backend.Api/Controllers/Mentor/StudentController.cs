using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Views.Mentor;

namespace Sefer.Backend.Api.Controllers.Mentor;

[Authorize(Roles = "Mentor")]
public class StudentController(IServiceProvider provider, IPasswordService passwordService)
    : Abstract.ProfileController(provider, passwordService)
{
    private readonly IFileStorageService _fileStorageService = provider.GetService<IFileStorageService>();

    private readonly IAvatarService _avatarService = provider.GetService<IAvatarService>();

    private readonly IShortUrlService _shortUrlService = provider.GetService<IShortUrlService>();

    private readonly ICryptographyService _cryptographyService = provider.GetService<ICryptographyService>();

    /// <summary>
    /// Returns an overview with all students of the given mentor
    /// </summary>
    /// <response code="403">The current user is not a mentor</response>
    [HttpGet("/mentor/students")]
    [ProducesResponseType(typeof(MentorStudentsView), 200)]
    public async Task<ActionResult<MentorStudentsView>> GetStudents()
    {
        // Check the mentor
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();

        // Find all students of this mentor
        var enrollments = await Send(new GetLastStudentEnrollmentsForMentorRequest(mentor.Id));

        var settings = await Send(new GetSettingsRequest());
        var active = DateTime.UtcNow.AddDays(-settings.StudentActiveDays);

        var view = new MentorStudentsView(enrollments, active, _avatarService);
        return Json(view);
    }

    [HttpGet("/mentor/students/{studentId:int}")]
    [ProducesResponseType(typeof(StudentOfMentorView), 200)]
    public async Task<ActionResult<StudentOfMentorView>> GetStudents(int studentId)
    {
        // Check if the student is a student of the mentor
        var (valid, mentor) = await ValidateRelationship(studentId);
        if (!valid) return Forbid();

        // Get the information of the student
        var info = await Send(new GetStudentInformationRequest(studentId));

        if (!info.HasValue) return StatusCode(418);
        var data = await Send(new GetStudentRemarksRequest(mentor.Id, studentId));

        var (student, current, isActive) = info.Value;

        // Now return the student
        var view = new StudentOfMentorView(student, isActive, mentor, data, current, _fileStorageService, _avatarService);
        return Json(view);
    }

    [HttpGet("/mentor/enrollments/{id:int}")]
    public async Task<ActionResult<MentorEnrollmentView>> GetEnrollment(int id)
    {
        // Load the enrollment from the database and check if it exists and if the mentor is requesting it
        var enrollment = await Send(new GetEnrollmentByIdExtensivelyRequest(id));
        if (enrollment == null || !enrollment.MentorId.HasValue) return NotFound();
        if (enrollment.MentorId != UserId) return Forbid();

        // And return the full view
        var data = await GetMentorStudentDataAsync(enrollment.MentorId.Value, enrollment.StudentId);
        var view = new MentorEnrollmentView(enrollment, _fileStorageService, data, _avatarService);
        return Json(view);
    }

    [HttpGet("/mentor/create-personal-invitation")]
    public async Task<ActionResult> CreatePersonalInvitation()
    {
        // Get and check the current user
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();
        var (_, mentorSite) = await Send(new GetPrimaryRegionAndSiteRequest(mentor.Id));

        // Create the url and qr-code for the mentor to share
        var expires = DateTime.UtcNow.AddHours(24);
        var queryString = _cryptographyService.TimeProtectedQueryString("pm", mentor.Id.ToString());
        var fullUrl = $"{mentorSite.SiteUrl}/register?{queryString}";
        var fallback = $"{mentorSite.SiteUrl}/registration-expired";

        var (shortUrl, qrCode) = await _shortUrlService.Create(fullUrl, expires, fallback);
        return Json(new PersonalInvitationView(qrCode, shortUrl, fullUrl));
    }

    [HttpPost("/mentor/student/remarks")]
    public async Task<IActionResult> SaveRemarks([FromBody] MentorRemarksPostModel post)
    {
        // Check if the student is a student of the mentor
        var (valid, mentor) = await ValidateRelationship(post.StudentId);
        if (!valid) return Forbid();

        // Save the remarks
        var data = await GetMentorStudentDataAsync(mentor.Id, post.StudentId);
        data.Remarks = post.Remarks;
        var saved = await Send(new SaveMentorStudentDataRequest(data));

        return saved ? Accepted() : BadRequest();
    }

    private async Task<(bool valid, User mentor)> ValidateRelationship(int studentId)
    {
        // Check the mentor
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return (false, mentor);

        // Check if the user is a student of a mentor
        var isStudent = await Send(new IsStudentOfMentorRequest(mentor.Id, studentId));
        return (isStudent, mentor);
    }

    private async Task<MentorStudentData> GetMentorStudentDataAsync(int mentorId, int studentId)
    {
        // Load existing data or create a new object
        return await Send(new GetStudentRemarksRequest(mentorId, studentId))
            ?? new MentorStudentData { MentorId = mentorId, StudentId = studentId };
    }
}