namespace Sefer.Backend.Api.Data.Handlers.Students;

public class RemoveStudentRoleHandler(IServiceProvider serviceProvider)
    : Handler<RemoveStudentRoleRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(RemoveStudentRoleRequest request, CancellationToken token)
    {
        var enrollments = await Send(new GetActiveEnrollmentsOfStudentRequest(request.StudentId), token);
        if (enrollments.Count == 0) return false;
        Cache.Remove("database-user-" + request.StudentId);
        
        var tasks = enrollments.Select(enrollment => Send(new UnEnrollRequest(enrollment.Id), token));
        var result = await Task.WhenAll(tasks);
        return !result.Contains(false);
    }
}