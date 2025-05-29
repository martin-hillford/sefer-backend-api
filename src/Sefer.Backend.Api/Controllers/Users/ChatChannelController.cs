using Sefer.Backend.Api.Services.Avatars;
using ChannelView = Sefer.Backend.Api.Views.Chat.ChannelView;

namespace Sefer.Backend.Api.Controllers.Users;

[Authorize(Roles = "Student,User,Admin,Mentor,Supervisor")]
public class ChatChannelController(IServiceProvider provider) : UserController(provider)
{
    private readonly IAvatarService _avatarService = provider.GetService<IAvatarService>();

    [HttpGet("/user/channels")]
    [ProducesResponseType(typeof(List<ChannelView>), 200)]
    public async Task<ActionResult> GetUserChannels()
    {
        // check if the user can be found (401)
        var user = await GetCurrentUser();
        if (user == null) return Unauthorized();

        // Load the channels from the user
        var channels = await Send(new GetChannelsRequest(user.Id));
        var unread = await Send(new GetUnreadMessageCountRequest(user.Id));

        // create the view
        var view = channels
            .Select(c =>
            {
                var count = unread.GetValueOrDefault(c.Id, 0);
                return new ChannelView(c, user, count, _avatarService);
            })
            .ToList();

        // Mentors have additional information in now if the channel is of an active user
        if (user.IsMentor) await AddUserActivityToChannel(view, user);
        return Json(view);
    }

    [HttpPost("/user/channels/admin")]
    [ProducesResponseType(typeof(Views.Shared.Users.Chat.ChannelView), 200)]
    public async Task<ActionResult> EnsuresPublicAdminChannel()
    {
        var user = await GetCurrentUser();
        if (user == null) return Unauthorized();

        var channel = await Send(new GetPublicAdminChannelRequest(user.Id));
        if (channel == null) return BadRequest();

        var view = new Views.Shared.Users.Chat.ChannelView(channel);
        return Json(view);
    }

    private async Task AddUserActivityToChannel(List<ChannelView> channels, User user)
    {
        var settings = await Send(new GetSettingsRequest());
        var active = DateTime.UtcNow.AddDays(-settings.StudentActiveDays);
        var activity = await Send(new GetActiveChannelsRequest(user.Id));
        
        foreach (var receiver in channels.SelectMany(channel => channel.Receivers))
        {
            if (!activity.TryGetValue(receiver.UserId, out var value)) continue;
            receiver.UserActive = value.ActivityDate > active;
        }
    }
}