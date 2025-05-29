namespace Sefer.Backend.Api.Controllers.Public;

public class NewsLetterController(IServiceProvider serviceProvider) : Support.UserController(serviceProvider)
{
    private readonly INewsletterService _newsLetterService = serviceProvider.GetService<INewsletterService>();

    /// <summary>
    /// Lets somebody subscribe to the newsletter
    /// </summary>
    /// <response code="400">Name or E-mail is not valid</response>
    [HttpPost("/newsletter/subscribe")]
    public async Task<ActionResult> Subscribe([FromBody] NewsletterSubscriptionPostModel data)
    {
        // Check if the name or e-mail is not valid
        if (!ModelState.IsValid) return BadRequest();

        // Use the newsletter solution connection to send data.
        var success = await _newsLetterService.Subscribe(data.Name, data.Email);

        // Return the result
        return success ? Ok() : StatusCode(500);
    }

    [HttpPost("/newsletter/is-subscribed")]
    [Authorize(Roles = "Student,User,Admin,Mentor,Supervisor")]
    public async Task<ActionResult> IsSubscribed()
    {
        var user = await GetCurrentUser();
        var result = await _newsLetterService.IsSubscribed(user.Email);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("/newsletter/unsubscribe")]
    [Authorize(Roles = "Student,User,Admin,Mentor,Supervisor")]
    public async Task<ActionResult> UnSubscribe()
    {
        var user = await GetCurrentUser();
        var result = await _newsLetterService.UnSubscribe(user.Email);
        return result ? Ok() : NotFound();
    }
}