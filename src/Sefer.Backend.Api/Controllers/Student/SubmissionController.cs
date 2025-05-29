namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class SubmissionController(IServiceProvider serviceProvider) : GrantController(serviceProvider)
{
    [HttpGet("/student/submissions/allowed")]
    public async Task<ActionResult> IsStudentAllowedToSubmit()
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        bool allowed;
        var active = await Send(new GetActiveEnrollmentOfStudentRequest(student.Id));
        if (active != null) allowed = await IsStudentAllowedToSubmit(student, active.CourseRevision.Course);
        else
        {
            var lowerBound = DateTime.UtcNow.Date.AddMilliseconds(-1);
            var upperBound = DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-1);
            var count = await Send(new GetSubmittedLessonsBetweenDatesCountRequest(lowerBound, upperBound, student.Id));
            var settings = await Send(new GetSettingsRequest());
            allowed = count - 1 < settings.MaxLessonSubmissionsPerDay;
        }

        if (allowed) return Ok();
        return NoContent();
    }

    [HttpPut("/student/lessons/current/audio-track/{audioTrack:int}")]
    public Task<ActionResult> SaveCurrentAudioTrack(int audioTrack)
        => SaveLessonSettings(null, audioTrack);
    
    [HttpPut("/student/lessons/current/page/{currentPage:int}")]
    public Task<ActionResult> SaveCurrentLessonPage(int currentPage)
        => SaveLessonSettings(currentPage, null);

    private async Task<ActionResult> SaveLessonSettings(int? currentPage, int? audioTrack)
    {
        // try to load the student that is updating his profile (404)
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return BadRequest();

        // gets the current lesson
        var (lesson, lessonSubmission, enrollment) = await Send(new GetCurrentLessonRequest(student.Id));
        if (lesson == null) return BadRequest();
        
        // Please note, pages are number with a one index
        // but audio tracks are zero index numbered

        if (lessonSubmission == null)
        {
            lessonSubmission = new LessonSubmission
            {
                CreationDate = DateTime.UtcNow,
                EnrollmentId = enrollment.Id,
                IsFinal = false,
                LessonId = lesson.Id,
                SubmissionDate = DateTime.UtcNow,
                CurrentPage = currentPage ?? 1,
                AudioTrack = currentPage ?? 0,
            };
            var inserted = await Send(new AddSubmissionRequest(lessonSubmission));
            if (!inserted) return StatusCode(500);
        }
        else
        {
            if (currentPage.HasValue) lessonSubmission.CurrentPage = currentPage.Value;
            if (audioTrack.HasValue) lessonSubmission.AudioTrack = audioTrack.Value;
            
            var updated = await Send(new UpdateSubmissionRequest(lessonSubmission));
            if (!updated) return StatusCode(500);
        }

        return Accepted();        
    }

    internal async Task<bool> IsStudentAllowedToSubmit(User student, Course course)
    {
        var settings = await Send(new GetSettingsRequest());
        if (settings.IsLessonSubmissionsLimited == false) return true;
        var submitted = await Send(new GetNumberOfSubmittedLessonsTodayRequest(student.Id));

        var courseSubmitted = course.MaxLessonSubmissionsPerDay;
        if (courseSubmitted.HasValue) return submitted < courseSubmitted.Value;
        return submitted < settings.MaxLessonSubmissionsPerDay;
    }
}