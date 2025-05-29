namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class EnrollHandler(IServiceProvider serviceProvider) : EntityHandler<EnrollRequest, Enrollment, Enrollment>(serviceProvider)
{
    public override async Task<Enrollment> Handle(EnrollRequest request, CancellationToken token)
    {
        // Check for the user
        var student = await Send(new GetUserByIdRequest(request.StudentId), token);
        if (student is not { IsStudent: true }) return null;

        // Check if the user has an active enrollment or already has completed the course
        var canEnroll = await Send(new IsStudentEnrollableForCourseRequest(request.StudentId, request.CourseId), token);
        if (canEnroll == false) return null;

        // Check if the course is actually published
        var courseRevision = await Send(new GetPublishedCourseRevisionRequest(request.CourseId), token);
        if (courseRevision == null) return null;

        // Create a new enrollment
        var enrollment = new Enrollment();

        // Get the mentor for this enrollment
        User mentor = null;
        if (!courseRevision.AllowSelfStudy)
        {
            mentor = await Send(new GetMentorAssigningRequest(request.CourseId, request.StudentId), token);
            if (mentor == null) return null;
            enrollment.MentorId = mentor.Id;
        }

        // create the enrollment
        enrollment.CourseRevisionId = courseRevision.Id;
        enrollment.IsCourseCompleted = false;
        enrollment.StudentId = request.StudentId;
        enrollment.CreationDate = DateTime.UtcNow;

        if (!await IsValid(enrollment)) return null;

        // Deal with retakes
        HandleRetakes(request.StudentId, request.CourseId);

        // Add the enrollment to the database
        var added = Add(enrollment);

        // Ensure to set some information for the caller
        if (!courseRevision.AllowSelfStudy) enrollment.Mentor = mentor;
        enrollment.CourseRevision = courseRevision;
        enrollment.Student = student;


        // Return the result
        return added ? enrollment : null;
    }

    private void HandleRetakes(int studentId, int courseId)
    {
        var context = GetDataContext();
        var retakes = context.Enrollments
            .Where(e => e.CourseRevision.CourseId == courseId && e.AllowRetake && e.StudentId == studentId)
            .ToList();

        retakes.ForEach(retake => retake.AllowRetake = false);
        context.SaveChanges();
    }

    private bool Add(Enrollment enrollment)
    {
        try
        {
            var context = GetDataContext();
            context.Enrollments.Add(enrollment);
            context.SaveChanges();
            return true;
        }
        catch (Exception) { return false; }
    }
}