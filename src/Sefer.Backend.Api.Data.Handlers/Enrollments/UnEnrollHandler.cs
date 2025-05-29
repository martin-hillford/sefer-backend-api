namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class UnEnrollHandler(IServiceProvider serviceProvider) : SyncHandler<UnEnrollRequest, bool>(serviceProvider)
{
    protected override bool Handle(UnEnrollRequest request)
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.SingleOrDefault(l => l.Id == request.EnrollmentId);
        if (enrollment == null || enrollment.IsActive == false) return false;
        if (enrollment.IsCourseCompleted) return false;

        enrollment.ClosureDate = DateTime.UtcNow;
        enrollment.IsCourseCompleted = false;
        enrollment.ModificationDate = DateTime.UtcNow;

        context.Enrollments.Update(enrollment);
        context.SaveChanges();
        return true;
    }
}