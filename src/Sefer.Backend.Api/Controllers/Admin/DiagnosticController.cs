using Sefer.Backend.Api.Models.Admin.Config;
using Sefer.Backend.Api.Notifications.Push;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class DiagnosticController(IServiceProvider provider) : BaseController(provider)
{
    private readonly IFireBaseService _firebaseService = provider.GetService<IFireBaseService>();

    [HttpPost("/admin/diagnostics/push-notifications")]
    public async Task<ActionResult> SendNotification([FromBody] NotificationPostModel post)
    {
        var path = ReadConfigFilePathAsync();
        try
        {
            if (ModelState.IsValid == false) return Ok(new { Success = false, ConfigFile = path, Message = "Invalid post" });
        
            var user = await Send(new GetUserByIdRequest(post.UserId));
            if (user == null) return Ok(new { Success = false, ConfigFile = path, Message = "User not found." });
            
            await _firebaseService.SendChatTextMessageNotification(post.UserId, post.Title , post.Content, true);
            return Ok(new { Success = true, ConfigFile = path });
        }
        catch (Exception ex)
        {
            return Ok(new { Success = false, ConfigFile = path, Error = ex.Message, Exception = ex.StackTrace });
        }
    }

    private string ReadConfigFilePathAsync()
    {
        var options = ServiceProvider.GetService<IOptions<FireBaseOptions>>();
        return string.IsNullOrEmpty(options?.Value.KeyFile) ? "/dev/null" : options.Value.KeyFile;
    }
}