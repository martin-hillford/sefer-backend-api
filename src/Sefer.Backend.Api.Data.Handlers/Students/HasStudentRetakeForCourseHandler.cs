namespace Sefer.Backend.Api.Data.Handlers.Students;

public class HasStudentRetakeForCourseHandler(IServiceProvider serviceProvider)
    : Handler<HasStudentRetakeForCourseRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(HasStudentRetakeForCourseRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Enrollments.AnyAsync
        (
            e => e.StudentId == request.StudentId &&
                 e.CourseRevision.CourseId == request.CourseId &&
                 e.IsCourseCompleted && e.AllowRetake,
            token
        );
    }
}