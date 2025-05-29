using Sefer.Backend.Api.Controllers.Mentor;
using Sefer.Backend.Api.Models.Admin.Enrollments;
using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Services.Security.Checksums;
using Sefer.Backend.Api.Views.Admin.Enrollment;
using Sefer.Backend.Api.Views.Mentor;
using Enrollment_StudentView = Sefer.Backend.Api.Views.Admin.Enrollment.StudentView;
using EnrollmentPostModel = Sefer.Backend.Api.Models.Admin.Enrollments.EnrollmentPostModel;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class EnrollmentController(IServiceProvider serviceProvider) : ReviewController(serviceProvider)
{
    private readonly ICryptographyService _cryptographyService = serviceProvider.GetService<ICryptographyService>();

    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();

    private readonly IAvatarService _avatarService = serviceProvider.GetService<IAvatarService>();

    [HttpGet("/admin/enrollments/student/{studentId:int}")]
    public async Task<ActionResult<Enrollment_StudentView>> GetEnrollmentsOfStudent(int studentId)
    {
        // Please note: if a student has been changed to a mentor the enrollments get still be viewed
        var student = await Send(new GetUserByIdRequest(studentId));
        if (student == null) return NotFound();
        var enrollments = await Send(new GetEnrollmentsOfStudentRequest(student.Id, null, true));
        var view = new Enrollment_StudentView(student, enrollments, _cryptographyService, _fileStorageService);
        return Json(view);
    }

    [HttpGet("/admin/enrollments/active")]
    public async Task<ActionResult<List<EnrollmentSummary>>> GetActiveEnrollments()
    {
        var enrollments = await Send(new GetActiveEnrollmentsExtensivelyRequest());
        return Json(enrollments);
    }

    [HttpPost("/admin/enrollments/change-mentor")]
    public async Task<ActionResult> ChangeMentor([FromBody] ChangeMentorPostModel model)
    {
        // Get the enrollment to update
        if (model == null) return BadRequest();
        var enrollment = await Send(new GetEnrollmentByIdRequest(model.EnrollmentId));
        if (enrollment == null) return NotFound();

        var oldMentor = await Send(new GetUserByIdRequest(enrollment.MentorId));
        var student = await Send(new GetUserByIdRequest(enrollment.StudentId));

        var courseRevision = await Send(new GetCourseRevisionByIdRequest(enrollment.CourseRevisionId));
        if (courseRevision == null) return BadRequest();
        var course = await Send(new GetCourseByIdRequest(courseRevision.CourseId));
        if (course == null) return BadRequest();
        var courseName = course.Name;

        // Check if a correct mentor is set
        var availableMentors = await Send(new GetMentorsRequest());
        var settings = await Send(new GetSettingsRequest());
        if (model.MentorId != settings.BackupMentorId && availableMentors.Any(m => m.Id == model.MentorId) == false) return BadRequest();

        if (enrollment.MentorId == model.MentorId) return NoContent();
        enrollment.MentorId = model.MentorId;
        var newMentor = await Send(new GetUserByIdRequest(model.MentorId));
        var success = await Send(new UpdateEnrollmentRequest(enrollment));
        if (success == false) return StatusCode(500);

        // Now send both to the new and old mentor a message
        var request = new PostMentorChangeChatMessageRequest
        {
            Student = student.Id,
            CourseName = courseName,
            NewMentor = newMentor.Id,
            OldMentor = oldMentor.Id
        };
        await Send(request);
        return NoContent();
    }

    [HttpPost("/admin/enrollments/unroll")]
    public async Task<ActionResult> UnrollStudent([FromBody] int enrollmentId)
    {
        // Get the enrollment to unroll
        var enrollment = await Send(new GetEnrollmentByIdRequest(enrollmentId));
        if (enrollment == null) return NotFound();

        var success = await Send(new UnEnrollRequest(enrollment.Id));
        return success ? NoContent() : StatusCode(500);
    }

    [HttpPost("/admin/enrollments/allow-retake")]
    public async Task<ActionResult> AllowRetake([FromBody] int enrollmentId)
    {
        // Get the enrollment to unroll
        var enrollment = await Send(new GetEnrollmentByIdRequest(enrollmentId));
        if (enrollment == null) return NotFound();

        var success = await Send(new AllowRetakeRequest(enrollmentId));
        return success ? NoContent() : StatusCode(500);
    }

    [HttpPost("/admin/enrollments")]
    public async Task<ActionResult> AddEnrollment([FromBody] EnrollmentPostModel posted)
    {
        // First check if the provided enrollment is valid
        if (!await IsValid(posted)) return BadRequest();

        // Determine the courseRevision
        var course = await Send(new GetCourseByIdRequest(posted.CourseId));
        if (course == null) return BadRequest();

        var revision = await Send(new GetPublishedCourseRevisionRequest(course.Id));
        if (revision == null)
        {
            var closed = await Send(new GetClosedCourseRevisionsRequest(course.Id));
            if (closed.Count == 0) revision = await Send(new GetEditingCourseRevisionRequest(course.Id));
            else revision = closed.OrderByDescending(e => e.Version).First();
        }

        // Now create the enrollment
        var enrollment = new Enrollment
        {
            AllowRetake = false,
            ClosureDate = posted.CompletionDate,
            CourseRevisionId = revision.Id,
            CreationDate = DateTime.UtcNow,
            IsCourseCompleted = posted.IsCompleted,
            MentorId = posted.MentorId,
            StudentId = posted.StudentId,
            SurveySubmitted = false,
            OnPaper = posted.OnPaper
        };
        if (posted.IsCompleted && posted.Grade.HasValue) enrollment.Grade = posted.Grade.Value / 10d;

        // Save the enrollment in the database
        var saved = await Send(new AddEnrollmentRequest(enrollment));

        enrollment.CourseRevision = revision;
        enrollment.CourseRevision.Course = course;

        await HandleGrants(enrollment);
        if (saved == false) return StatusCode(500);
        var view = new { EnrollmentId = enrollment.Id };
        return Json(view, 201);
    }

    [HttpGet("/admin/enrollments/{enrollmentId:int}")]
    public async Task<ActionResult> GetSubmissionsOfEnrollment(int enrollmentId)
    {
        // Load the enrollment with everything that is required
        var enrollment = await Send(new GetEnrollmentByIdExtensivelyRequest(enrollmentId));
        if (enrollment == null) return NotFound();

        // Load the details on the submission
        foreach (var submission in enrollment.LessonSubmissions)
        {
            submission.Lesson.Content = await Send(new GetLessonContentRequest(submission.Lesson));
        }

        var view = new EnrollmentDetailView(enrollment, _cryptographyService, _fileStorageService, _avatarService);
        return Json(view);
    }

    [HttpGet("/admin/enrollments/submissions/{submissionId:int}")]
    public async Task<ActionResult> GetSubmissionsById(int submissionId)
    {
        // First get the mentor and the submission selected
        var admin = await GetCurrentUser();
        var submission = await Send(new GetSubmissionForReviewByIdRequest(admin.Id, submissionId));
        if (submission == null) return NotFound();

        var view = SubmissionsController.GetAnswersForSubmissionView(submission);
        return Json(view);
    }

    [HttpPost("/admin/enrollments/review")]
    public async Task<ActionResult> ReviewSubmissionForMentor([FromBody] SubmissionReviewPostModel review)
    {
        // Get the admin that has preformed the review
        var admin = await GetCurrentUser();

        // Get the result and return (leverage on mentor functionality)
        var saved = await ReviewSubmission(review, admin);
        return saved ? Ok() : BadRequest();
    }

    [HttpGet("/admin/enrollments/review")]
    public async Task<ActionResult<List<ReviewSummaryView>>> GetOpenSubmissions()
    {
        var admin = await GetCurrentUser();
        var submissions = await Send(new GetSubmissionsForReviewRequest(admin.Id));
        var view = submissions.Select(s => new ReviewSummaryView(s, null, _avatarService)).ToList();
        return Json(view);
    }

    [HttpGet("/admin/enrollments/verify/{code}")]
    public async Task<ActionResult<EnrollmentVerificationView>> VerifyEnrollment(string code)
    {
        var enrollmentId = EnrollmentChecksum.GetEnrollmentId(code);
        if (enrollmentId == null) return NotFound();

        var enrollment = await Send(new GetEnrollmentByIdExtensivelyRequest(enrollmentId));
        if (enrollment == null || enrollment.IsCourseCompleted == false || enrollment.Grade.HasValue == false || (enrollment.Grade * 10) < 7) return NotFound();
        var view = new EnrollmentVerificationView(enrollment, _cryptographyService);
        return Json(view);
    }

    private async Task<bool> IsValid(EnrollmentPostModel enrollment)
    {
        if (enrollment == null) return false;
        switch (enrollment.IsCompleted)
        {
            case true when enrollment.Grade is < 1:
            case true when enrollment.Grade is > 10:
            case true when enrollment.CompletionDate.HasValue == false:
                return false;
        }

        var student = await Send(new GetUserByIdRequest(enrollment.StudentId));
        if (student == null) return false;

        var course = await Send(new GetCourseByIdRequest(enrollment.CourseId));
        if (course == null) return false;

        if (enrollment.MentorId == null) return true;

        var mentor = await Send(new GetUserByIdRequest(enrollment.MentorId));
        return mentor?.IsMentor == true;
    }
}