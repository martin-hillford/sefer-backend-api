using Sefer.Backend.Api.Data.Requests.Rewards;
using Sefer.Backend.Api.Views.Student;

namespace Sefer.Backend.Api.Support;

public abstract class GrantController(IServiceProvider provider) : UserController(provider)
{
    protected async Task<List<GrantView>> HandleGrants(Enrollment enrollment)
    {
        var notificationService = GetService<INotificationService>();

        var rewards = await Send(new SubmitForGrantRequest(enrollment.Id));
        if (rewards == null || rewards.Count == 0) return new List<GrantView>();

        var student = await Send(new GetUserByIdRequest(enrollment.StudentId));
        await notificationService.SendRewardReceivedNotificationAsync(student, rewards);

        var view = rewards.Select(g => new GrantView(g)).ToList();
        return view;
    }
}