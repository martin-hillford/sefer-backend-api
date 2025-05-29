namespace Sefer.Backend.Api.Data.Handlers.Students;

public class HasStudentCompletedCourseHandler(IServiceProvider serviceProvider)
    : Handler<HasStudentCompletedCourseRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(HasStudentCompletedCourseRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Enrollments.AnyAsync
        (
            e => e.StudentId == request.StudentId &&
                 e.CourseRevision.CourseId == request.CourseId &&
                 e.IsCourseCompleted,
            token
        );
    }
}