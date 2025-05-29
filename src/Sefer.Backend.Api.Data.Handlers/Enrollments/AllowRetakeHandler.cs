namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class AllowRetakeHandler(IServiceProvider serviceProvider) : Handler<AllowRetakeRequest, bool>(serviceProvider)
{
    public override Task<bool> Handle(AllowRetakeRequest request, CancellationToken token)
    {
        var result = HandleSync(request);
        return Task.FromResult(result);
    }

    private bool HandleSync(AllowRetakeRequest request)
    {
        // First load both the course and student in the enrollment
        using var context = GetDataContext();
        var enrollment = context.Enrollments.SingleOrDefault(e => e.Id == request.EnrollmentId);
        if (enrollment == null) return false;

        context.Entry(enrollment).Reference(e => e.CourseRevision).Load();

        var course = context.Courses.SingleOrDefault(e => e.Id == enrollment.CourseRevision.CourseId);
        if (course == null) return false;

        var student = context.Users.SingleOrDefault(u => u.Id == enrollment.StudentId);
        if (student == null) return false;

        // Now load all the completed courses of the student and update them
        var enrollments = context.Enrollments
            .Where(e => e.StudentId == student.Id &&
                        e.CourseRevision.CourseId == course.Id &&
                        e.IsCourseCompleted)
            .ToListThenForEach(retake => { retake.AllowRetake = true; });

        context.UpdateRange(enrollments);
        context.SaveChanges();
        return true;
    }
}