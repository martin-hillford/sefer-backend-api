namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class IsStudentOfMentorHandler(IServiceProvider serviceProvider)
    : Handler<IsStudentOfMentorRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsStudentOfMentorRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Enrollments
            .AnyAsync(e => e.StudentId == request.StudentId && e.MentorId == request.MentorId, token);
    }
}