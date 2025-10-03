using Sefer.Backend.Api.Models.Users;
using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Services.Avatars.ThirdParty;

namespace Sefer.Backend.Api.Controllers.Users;

public class AvatarController(IServiceProvider provider) : UserController(provider)
{
    private readonly AvatarOptions _avatarOptions = provider.GetService<IOptions<AvatarOptions>>().Value;
    
    private readonly IAvatarService _avatarService =  provider.GetService<IAvatarService>();
    
    [ResponseCache(Duration = 3600)]
    [HttpGet("/avatars/avatar")]
    [HttpGet("/avatar")]
    public Task<IActionResult> GetAvatarCached(string hash, string initials, string fill, string color)
        => GetAvatar(hash, initials, fill, color);
    
    [ResponseCache(Duration = 0)]
    [HttpGet("/avatars/avatar-no-cache")]
    [HttpGet("/avatar-no-cache")]
    public Task<IActionResult> GetAvatarUnCached(string hash, string initials, string fill, string color)
        => GetAvatar(hash, initials, fill, color);
    
    private async Task<IActionResult> GetAvatar(string hash, string initials,  string fill, string color)
    {
        // Check if an avatar is present in the cache
        var cache = new Cache(_avatarOptions);
        var cached = await cache.FromCache(hash);
        if (cached.IsCached && cached.Response is { HasImage: true }) return cached.Response.Send();

        // Create a fallback avatar
        return Unknown.Create(initials, fill, color).Send();
    }

    [Authorize(Roles = "Student,User,Admin,Mentor,Supervisor")]
    [HttpPost("/user/avatar")]
    public async Task<ActionResult> UploadAvatar([FromBody] AvatarPostModel body)
    {
        try
        {
            if (!ModelState.IsValid || body == null || UserId == null) return BadRequest();
        
            // And save the image to the cache
            var cache = new Cache(_avatarOptions);
            var response = Services.Avatars.Response.FromBase64(body.Image, body.Type);
            var hash = _avatarService.GetAvatarId(UserId.Value);
            await cache.Store(hash, response);
            return Ok();
        }
        catch{ return Problem("An exception occured while uploading the avatar"); }   
    }

    [Authorize(Roles = "Student,User,Admin,Mentor,Supervisor")]
    [HttpGet("/avatars/third-party")]
    public async Task<IActionResult> GetExternalAvatar()
    {
        var user = await GetCurrentUser();
        if (user == null) return NotFound();
        var client = ServiceProvider.GetService<IHttpClient>();
        var external = new External(client);
        var response = await external.Retrieve(user.Email, 120);
        return Ok(response ?? Services.Avatars.Response.Empty());
    }
}