using Sefer.Backend.Api.Models.Users;

namespace Sefer.Backend.Api.Controllers.Users;

[Authorize(Roles = "Student,User,Mentor,Supervisor")]
public class ImpersonationController(IServiceProvider provider) : UserController(provider)
{
    [HttpPost("/user/impersonation")]
    [ProducesResponseType(202)]
    public async Task<IActionResult> SetImpersonation([FromBody] ImpersonationPostModel post)
    {
        // check the input (400)
        if (post == null || ModelState.IsValid == false) return BadRequest();

        // check if the user can be found (401)
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();

        // Now update the user settings
        user.AllowImpersonation = post.AllowImpersonation;
        await Send(new UpdateUserRequest(user));

        return Ok();
    }
}