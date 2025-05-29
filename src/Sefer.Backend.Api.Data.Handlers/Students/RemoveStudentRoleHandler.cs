namespace Sefer.Backend.Api.Data.Handlers.Students;

public class RemoveStudentRoleHandler(IServiceProvider serviceProvider)
    : Handler<RemoveStudentRoleRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(RemoveStudentRoleRequest request, CancellationToken token)
    {
        var enrollment = await Send(new GetActiveEnrollmentOfStudentRequest(request.StudentId), token);
        if (enrollment == null) return false;
        Cache.Remove("database-user-" + request.StudentId);
        return await Send(new UnEnrollRequest(enrollment.Id), token);
    }
}