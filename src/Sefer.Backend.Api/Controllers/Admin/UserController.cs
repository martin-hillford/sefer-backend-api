namespace Sefer.Backend.Api.Controllers.Admin;

/// <summary>
/// This controller deals with all the course requests from an admin
/// </summary>
[Authorize(Roles = "Admin")]
public class UserController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    /// <summary>
    /// The service to validate user authentication
    /// </summary>
    private readonly IUserAuthenticationService _userAuthenticationService = serviceProvider.GetService<IUserAuthenticationService>();

    [HttpGet("/users/students")]
    [ProducesResponseType(typeof(List<UserListView>), 200)]
    public async Task<IActionResult> GetStudents()
    {
        var students = await Send(new GetUsersByRoleRequest(UserRoles.Student));
        var active = await Send(new GetActiveStudentsRequest());
        var view = students.Select(s => new StudentListView(s, active.Contains(s.Id)));
        return Json(view);
    }

    [HttpGet("/users/administrators")]
    [ProducesResponseType(typeof(List<MentorListView>), 200)]
    public async Task<IActionResult> GetAdministrators() => await GetMentors(UserRoles.Admin);

    [HttpGet("/users/mentors")]
    [ProducesResponseType(typeof(List<MentorListView>), 200)]
    public async Task<IActionResult> GetMentors() => await GetMentors(UserRoles.Mentor);

    [HttpGet("/users/all-mentors")]
    [ProducesResponseType(typeof(List<UserListView>), 200)]
    public async Task<IActionResult> GetAllMentors()
    {
        var mentors = await Send(new GetMentorsRequest());
        var settings = await Send(new GetSettingsRequest());
        var view = new List<UserListView>();
        mentors.ForEach(u => view.Add(new UserListView(u)));

        // Check if the backup mentor must be added
        if (view.Any(v => v.Id == settings.BackupMentorId)) return Json(view);

        var backupMentor = await Send(new GetUserByIdRequest(settings.BackupMentorId));
        if (backupMentor != null) view.Add(new UserListView(backupMentor));
        return Json(view);
    }

    [HttpGet("/users/supervisors")]
    [ProducesResponseType(typeof(List<MentorListView>), 200)]
    public async Task<IActionResult> GetSupervisor() => await GetMentors(UserRoles.Supervisor);

    [HttpGet("/users")]
    [ProducesResponseType(typeof(List<UserListView>), 200)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await Send(new GetUsersRequest());
        var view = new List<UserListView>();
        users.ForEach(u => view.Add(new UserListView(u)));
        return Json(view);
    }

    /// <summary>
    /// This method can be used to block (or unblock) users
    /// </summary>
    /// <param name="blocked">True when the user should be blocked, false when unblocked</param>
    /// <param name="userId">The id of the user to block/unblock</param>
    /// <returns></returns>
    /// <response code="204">The user is blocked</response>
    /// <response code="404">The user could not be found</response>
    [HttpPost("/users/{userId:int}/block")]
    public async Task<ActionResult> BlockUser([FromBody] bool blocked, int userId)
    {
        var user = await Send(new GetUserByIdRequest(userId));
        if (user == null) return NotFound();

        if (userId == _userAuthenticationService.UserId) return BadRequest();

        user.Blocked = blocked;
        await Send(new UpdateSingleUserPropertyRequest(user, "Blocked"));

        return StatusCode(204);
    }

    [HttpPost("/users/{userId:int}/approve")]
    public async Task<ActionResult> ApproveUser(int userId)
    {
        var user = await Send(new GetUserByIdRequest(userId));
        if (user == null) return NotFound();

        user.Approved = true;
        await Send(new UpdateSingleUserPropertyRequest(user, nameof(user.Approved)));
        return StatusCode(204);
    }

    [HttpPost("/users/role")]
    public async Task<ActionResult> ChangeUserRole([FromBody] ChangeRolePostModel post)
    {
        var user = await Send(new GetUserByIdRequest(post.UserId));
        if (user == null) return NotFound();

        if (user.Role == UserRoles.Admin && post.Role != UserRoles.Admin)
        {
            var settings = await Send(new GetSettingsRequest());
            if (user.Id == settings.BackupMentorId) return StatusCode(204);
        }

        if (user.Id == _userAuthenticationService.UserId) return BadRequest();

        var changed = await Send(new UpdateUserRoleRequest(user.Id, post.Role));
        return changed ? StatusCode(202) : BadRequest();
    }

    [HttpPost("/users/disable-two-factor-auth")]
    public async Task<ActionResult> DisableTwoFactorAuth([FromBody] DisableTwoAuthPostModel post)
    {
        var user = await Send(new GetUserByIdRequest(post.UserId));
        if (user == null) return NotFound();

        var notificationService = ServiceProvider.GetService<INotificationService>();
        var disabled = await Send(new DisableTwoFactorAuthRequest(user.Id));
        var language = user.GetPreferredInterfaceLanguage();
        if (disabled) await notificationService.SendTwoFactorAuthDisabledNotificationAsync(user, language);
        return disabled ? StatusCode(202) : BadRequest();
    }

    [HttpPost("/users/primary-region-site")]
    public async Task<ActionResult> ChangeUserPrimarySite([FromBody] ChangePrimarySitePostModel post)
    {
        var site = await Send(new GetSiteByNameRequest(post.Site));
        var region = await Send(new GetRegionByIdRequest(post.Region));
        if (site == null || !site.ContainsRegion(region)) return BadRequest();
        var changed = await Send(new UpdateUserPrimarySiteRequest(post.UserId, site, region));
        return changed ? StatusCode(202) : BadRequest();
    }

    private async Task<IActionResult> GetMentors(UserRoles role)
    {
        var users = await Send(new GetMentorsWithSettingsRequest(role));
        var activeStudents = await Send(new GetActiveStudentsOfMentorsRequest());
        var ratings = await Send(new GetMentorRatingsRequest(role));
        var view = new List<MentorListView>();

        users.ForEach(u =>
        {
            var numberActiveStudents = activeStudents.GetActiveStudents(u.Id);
            var hasRating = ratings.ContainsKey(u.Id);

            var rating = hasRating ? ratings[u.Id].Item1 : 0;
            var ratingCount = hasRating ? ratings[u.Id].Item2 : 0;

            view.Add(new MentorListView(u, numberActiveStudents, ratingCount, rating));
        });
        return Json(view);
    }

    [HttpGet("/users/{userId:int}/current-admin-channel")]
    public async Task<IActionResult> EnsureChatChannel(int userId)
    {
        var user = await Send(new GetUserByIdRequest(userId));
        var admin = await Send(new GetUserByIdRequest(_userAuthenticationService?.UserId));

        if (user == null || admin == null) return BadRequest();

        var channel = await Send(new GetPersonalChannelRequest(user.Id, admin.Id));
        if (channel == null) return NoContent();

        var view = new ChannelView(channel);
        return Json(view);
    }
}