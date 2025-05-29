namespace Sefer.Backend.Api.Data.Handlers.Users;

public class UpdateUserRoleHandler(IServiceProvider serviceProvider)
    : Handler<UpdateUserRoleRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(UpdateUserRoleRequest request, CancellationToken token)
    {
        // Try to load the user
        var user = await Send(new GetUserByIdRequest(request.UserId), token);
        if (user == null) return false;

        // Ensure to clean the cache for the role of user is not cached
        Cache.Remove("database-user-" + request.UserId);

        // if the role does not change return true
        if (user.Role == request.Role) return true;

        // Deal with degrading for the supervisor
        if (user.IsSupervisor) await HandleSupervisor(user.Id, request.Role, token);

        // Deal with degrading for the mentor
        if (user.IsMentor) await HandleMentor(user.Id, request.Role, token);

        // Deal with changing role of the student
        if (user.Role == UserRoles.Student) await Send(new RemoveStudentRoleRequest(user.Id), token);

        // return when the user is updated
        user.Role = request.Role;
        var updated = await Send(new UpdateSingleUserPropertyRequest(user, nameof(user.Role)), token);
        if (!updated) return false;

        // Ensure when the mentor is a mentor that the user has settings
        if (user.IsMentor) await Send(new EnsureMentorSettingsRequest(user.Id), token);

        return true;
    }

    private async Task HandleMentor(int mentorId, UserRoles role, CancellationToken token)
    {
        if (role is not (UserRoles.User or UserRoles.Student)) return;
        await Send(new RemoveAsMentorRequest(mentorId), token);
    }

    private async Task HandleSupervisor(int supervisorId, UserRoles role, CancellationToken token)
    {
        if (role is UserRoles.Admin or UserRoles.Supervisor) return;
        await Send(new RemoveAsSupervisorRequest(supervisorId), token);
    }
}