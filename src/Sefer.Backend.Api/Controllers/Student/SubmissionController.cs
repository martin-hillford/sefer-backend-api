namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class SubmissionController(IServiceProvider serviceProvider) : GrantController(serviceProvider)
{
    /// <summary>
    /// Check if the student is allowed to submit a submission
    /// </summary>
    [HttpGet("/student/enrollments/{enrollmentId:int}/submissions/allowed")]
    public async Task<ActionResult> IsStudentAllowedToSubmit(int enrollmentId)
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();
        
        var enrollments = await Send(new GetActiveEnrollmentsOfStudentRequest(student.Id));
        var selected = enrollments.FirstOrDefault(e => e.Id == enrollmentId);
        if(selected == null) return NotFound();
        return await IsStudentAllowedToSubmit(student, selected);
    }
    
    /// <summary>
    /// Check if the student is allowed to submit a submission
    /// </summary>
    /// <returns>Please note, this endpoint can only be used for we</returns>
    [HttpGet("/student/submissions/allowed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> IsStudentAllowedToSubmit()
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        // Note this is endpoint can only be used so single enrollment installations
        var settings = await Send(new GetSettingsRequest());
        if (settings.AllowMultipleActiveEnrollments) return StatusCode(418);          
        
        var enrollments = await Send(new GetActiveEnrollmentsOfStudentRequest(student.Id));
        return await IsStudentAllowedToSubmit(student, enrollments.FirstOrDefault());
    }

    private async Task<ActionResult> IsStudentAllowedToSubmit(User student, Enrollment enrollment)
    {
        bool allowed;
        if (enrollment != null) allowed = await IsStudentAllowedToSubmit(student, enrollment.CourseRevision.Course);
        else
        {
            var lowerBound = DateTime.UtcNow.Date.AddMilliseconds(-1);
            var upperBound = DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-1);
            var count = await Send(new GetSubmittedLessonsBetweenDatesCountRequest(lowerBound, upperBound, student.Id));
            var settings = await Send(new GetSettingsRequest());
            allowed =
                !settings.IsLessonSubmissionsLimited ||
                count - 1 < settings.MaxLessonSubmissionsPerDay;
        }

        if (allowed) return Ok();
        return NoContent();
    }

    internal async Task<bool> IsStudentAllowedToSubmit(User student, Course course)
    {
        // Get the number of currently submitted lessons today
        var submitted = await Send(new GetNumberOfSubmittedLessonsTodayRequest(student.Id));
        
        // First check if a specific number of lessons is set by the course
        var courseSubmitted = course.MaxLessonSubmissionsPerDay;
        if (courseSubmitted  is > 0) return submitted < courseSubmitted.Value;
        
        // Next test the settings
        var settings = await Send(new GetSettingsRequest());
        if (!settings.IsLessonSubmissionsLimited) return true;
        return submitted < settings.MaxLessonSubmissionsPerDay;
    }
    
    [HttpPut("/student/lessons/current/audio-track/{audioTrack:int}")]
    public async Task<ActionResult> SaveCurrentAudioTrack(int audioTrack)
    {
        // Note this is endpoint can only be used so single enrollment installations
        var settings = await Send(new GetSettingsRequest());
        if (settings.AllowMultipleActiveEnrollments) return StatusCode(418);
        
        return await SaveLessonSettings(null, null, audioTrack);
    }
    
    [HttpPut("/student/enrollments/{enrollmentId:int}/audio-track/{audioTrack:int}")]
    public Task<ActionResult> SaveCurrentAudioTrack(int enrollmentId, int audioTrack)
        => SaveLessonSettings(enrollmentId, null, audioTrack);
    
    [HttpPut("/student/lessons/current/page/{currentPage:int}")]
    public async Task<ActionResult> SaveCurrentLessonPage(int currentPage)
    {
        // Note this is endpoint can only be used so single enrollment installations
        var settings = await Send(new GetSettingsRequest());
        if (settings.AllowMultipleActiveEnrollments) return StatusCode(418);
        
        return await SaveLessonSettings(null, currentPage, null);   
    }
    
    [HttpPut("/student/enrollments/{enrollmentId:int}/page/{currentPage:int}")]
    public Task<ActionResult> SaveCurrentLessonPage(int enrollmentId, int currentPage)
        => SaveLessonSettings(enrollmentId, currentPage, null);
    
    private async Task<ActionResult> SaveLessonSettings(int? enrollmentId, int? currentPage, int? audioTrack)
    {
        // try to load the student that is updating his profile (404)
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return BadRequest();
        
        // Check if there is an active enrollment.
        var active = await GetActiveEnrollment(student, enrollmentId);
        if(active == null) return BadRequest();

        // gets the current lesson
        var (lesson, lessonSubmission, enrollment) = await Send(new GetCurrentLessonRequest(student.Id, active.Id));
        if (lesson == null) return BadRequest();
        
        // Update the user took some action
        await Send(new UpdateUserLastActivityRequest(enrollment.StudentId));
        
        // Please note, pages are one index numbered, but audio tracks are zero index numbered
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
    
    private async Task<Enrollment> GetActiveEnrollment(User student, int? enrollmentId)
    {
        // get the current enrollments of the student
        var enrollments = await Send(new GetActiveEnrollmentsOfStudentRequest(student.Id));
        
        // When multiple active enrollments are allowed, the client must post the enrollmentId
        var settings = await Send(new GetSettingsRequest());
        if (settings.AllowMultipleActiveEnrollments && !enrollmentId.HasValue) return null;

        // Only one active enrollment
        return enrollmentId.HasValue
            ? enrollments.FirstOrDefault(e => e.Id == enrollmentId.Value)
            : enrollments.FirstOrDefault();
    }
}