namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class CompleteEnrollmentHandler(IServiceProvider serviceProvider)
    : Handler<CompleteEnrollmentRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(CompleteEnrollmentRequest request, CancellationToken token)
    {
        var grade = await GetGradeOfEnrollment(request.EnrollmentId, token);

        await using var context = GetDataContext();
        var enrollment = context.Enrollments.SingleOrDefault(e => e.Id == request.EnrollmentId);

        if (enrollment == null || enrollment.IsActive == false) return false;
        if (enrollment.IsCourseCompleted) return true;

        enrollment.ClosureDate = DateTime.UtcNow;
        enrollment.IsCourseCompleted = true;
        enrollment.ModificationDate = DateTime.UtcNow;
        enrollment.Grade = grade;

        context.Enrollments.Update(enrollment);
        context.SaveChanges();
        return true;
    }

    private async Task<double?> GetGradeOfEnrollment(int enrollmentId, CancellationToken token)
    {
        var context = GetDataContext();
        var lessons = context.LessonSubmissions.Where(l => l.EnrollmentId == enrollmentId);
        if (await lessons.AnyAsync(l => l.Grade == null || l.IsFinal == false, token)) return null;
        return lessons.Sum(l => l.Grade.Value) / lessons.Count();
    }
}