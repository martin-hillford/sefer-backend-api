using Sefer.Backend.Api.Models.Users;
using Sefer.Backend.Api.Services.Avatars;

namespace Sefer.Backend.Api.Controllers.Users;

[Authorize(Roles = "Student,User,Admin,Mentor,Supervisor")]
public class AvatarController(IServiceProvider provider) : UserController(provider)
{
    private readonly IHttpClient _httpClient = provider.GetService<IHttpClient>();

    private readonly IAvatarService _avatarService = provider.GetService<IAvatarService>();

    /// <summary>
    /// Uploads the avatar
    /// </summary>
    /// <response code="400">The image is not set or the user could not be found</response>
    /// <response code="500">An error occurred while uploading</response>
    /// <response code="204">The image is uploaded</response>
    [HttpPost("/user/avatar")]
    public async Task<ActionResult> UploadAvatar([FromBody] AvatarPostModel body)
    {
        if (!ModelState.IsValid || body == null || UserId == null) return BadRequest();
        var uploadUrl = _avatarService.GetAvatarUploadUrl(UserId.Value);

        var response = await _httpClient.PostAsJsonAsync(uploadUrl, body);

        if (!response.IsSuccessStatusCode) return new StatusCodeResult(500);
        return NoContent();
    }
}