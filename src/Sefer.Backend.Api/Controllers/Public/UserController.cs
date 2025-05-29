using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Services.Extensions;
using Sefer.Backend.Api.Views.Shared;

namespace Sefer.Backend.Api.Controllers.Public;

public class UserController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly IPasswordService _passwordService = serviceProvider.GetService<IPasswordService>();

    private readonly ICryptographyService _cryptoService = serviceProvider.GetService<ICryptographyService>();

    private readonly INotificationService _notificationService = serviceProvider.GetService<INotificationService>();

    [HttpPost("/public/user/registrations/email")]
    [ProducesResponseType(typeof(BooleanView), 200)]
    public async Task<IActionResult> IsEmailUnique([FromBody] IsEmailUniquePostModel post)
    {
        if (post == null) return BadRequest();
        var isEmailUnique = await IsEmailUnique(post.Email);
        var view = new BooleanView { Response = isEmailUnique };
        return Json(view);
    }

    [HttpPost("/public/user/registrations")]
    [ProducesResponseType(typeof(UserView), 201)]
    public async Task<ActionResult> Registration([FromBody] RegistrationPostModel post)
    {
        // If the model state is not valid, the post is valid, bad request
        if (post == null) return BadRequest();
        if (ModelState.IsValid == false) return BadRequest();

        // Check if a user can be enrolled without a unique address
        if (!await IsEmailUnique(post.Email)) return BadRequest();

        // Get the site of the user
        var site = await Send(new GetSiteByNameRequest(post.Site));
        var region = await Send(new GetRegionByIdRequest(post.Region));
        if (!site.ContainsRegion(region)) return StatusCode(500);

        // Create a new user and use the password service to set the password
        var user = new User
        {
            Active = false,
            Approved = false,
            Blocked = false,
            Email = post.Email,
            Gender = post.Gender,
            Name = post.Name,
            Role = UserRoles.User,
            YearOfBirth = post.YearOfBirth,
            SubscriptionDate = DateTime.UtcNow,
            NotificationPreference = NotificationPreference.Direct,
            PreferredInterfaceLanguage = post.Language,
            PrimarySite = site.Hostname,
            PrimaryRegion = region.Id
        };
        _passwordService.UpdatePassword(user, post.Password);

        // Check if the user is valid, and save it
        var isValid = await Send(new AddUserRequest(user));
        if (isValid == false) return BadRequest();

        // Check if this registration is from a personal mentor invitation
        if (IsRegisteringWithMentorInvitation(post))
        {
            var mentorId = post.Invitation.GetMentorId();
            await Send(new SetPersonalMentorRequest(user.Id, mentorId));
        }

        // Now send an e-mail to the user to confirm his/hers account
        var notify = _notificationService.SendCompleteRegistrationNotificationAsync;
        if (post.IsInAppRegistration) notify = _notificationService.SendCompleteInAppRegistrationNotificationAsync;
        await notify(user, post.Language);

        // Done let the user know  it is done
        return Json(new UserView(user), 201);
    }

    [HttpPost("/public/user/activate")]
    [ProducesResponseType(200)]
    public async Task<ActionResult> Activate([FromBody] ActivationPostModel activation)
    {
        if (activation == null) return BadRequest();

        var valid = _cryptoService.IsProtectedQueryString(activation.User.ToString(), activation.Random, activation.Hash);
        if (valid == false) return BadRequest();

        var user = await Send(new GetUserByIdRequest(activation.User));
        if (user == null) return BadRequest();

        user.Approved = true;
        await Send(new UpdateSingleUserPropertyRequest(user, nameof(user.Approved)));
        return Ok();
    }

    [HttpPost("/public/user/activate-app")]
    [ProducesResponseType(200)]
    public async Task<ActionResult> ActivateApp([FromBody] AppActivationPostModel data)
    {
        // Check the incoming data
        if (data == null || ModelState.IsValid == false) return BadRequest();
        var user = await Send(new GetUserByEmailRequest(data.Email));
        if (user == null) return BadRequest();

        var expected = _cryptoService.GetUserActivationCode(user);
        user.Approved = (data.Code?.ToLower() ?? string.Empty).Equals(expected, StringComparison.CurrentCultureIgnoreCase);

        if (user.Approved) await Send(new UpdateSingleUserPropertyRequest(user, nameof(user.Approved)));
        return user.Approved ? Ok() : BadRequest();
    }

    [HttpPost("/public/user/update-email")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> UpdateEmailAddress([FromBody] UpdateEmailPostModel post)
    {
        try
        {
            // Check the incoming data
            if (post == null || ModelState.IsValid == false) return BadRequest();

            // Determine if the provided link is correct (400)
            var validLink = _cryptoService.IsTimeProtectedQueryString(post);
            if (validLink == false) return BadRequest();

            // decrypted the data (400)
            var decryptedData = _cryptoService.Decrypt(post.Data).Split('-', StringSplitOptions.RemoveEmptyEntries);
            var userId = int.Parse(decryptedData[0]);
            var newEmail = decryptedData[1];

            // check the decrypted data (400)
            if (newEmail.ToLower().Trim() != post.New.ToLower().Trim()) return BadRequest();
            var user = await Send(new GetUserByIdRequest(userId));
            if (user == null) return BadRequest();

            // check the user e-mail / password (403)
            if (user.Email.ToLower().Trim() != post.Email.ToLower().Trim()) return Forbid();
            if (_passwordService.IsValidPassword(user, post.Password) == false) return Forbid();

            // Everything is valid, update the user
            user.Email = newEmail;
            await Send(new UpdateSingleUserPropertyRequest(user, nameof(user.Email)));

            // Send the confirmation e-mail (to both the old and new address)
            await _notificationService.SendEmailUpdateCompleteNotificationAsync(user, post.Language, newEmail, post.Old);

            // The system is done
            return StatusCode(202);
        }
        catch (Exception) { return BadRequest(); }
    }

    [HttpPost("/public/student/delete-account")]
    public async Task<ActionResult> DeleteAccount([FromBody] RemoveAccountPostModel post)
    {
        try
        {
            // Check the incoming data
            if (post == null || ModelState.IsValid == false) return BadRequest();

            // Determine if the provided link is correct (400)
            var validLink = _cryptoService.IsTimeProtectedQueryString(post);
            if (validLink == false) return BadRequest();

            // Get the student id
            var studentId = int.Parse(post.User);

            // check the decrypted data (400)
            var user = await Send(new GetUserByIdRequest(studentId));
            if (user == null || user.IsStudent == false) return BadRequest();

            var deleted = await Send(new DeleteStudentRequest(studentId));
            return StatusCode(deleted == false ? 500 : 202);
        }
        catch (Exception) { return BadRequest(); }
    }

    private bool IsRegisteringWithMentorInvitation(RegistrationPostModel registration)
    {
        if (registration?.Invitation == null) return false;

        var (data, random, date, hash) = registration.Invitation.GetParams();
        return
            _cryptoService.IsTimeProtectedQueryString(data, random, date, hash) &&
            registration.Invitation.GetMentorId() > 0;
    }

    private async Task<bool> IsEmailUnique(string email)
        => await Send(new GetUserByEmailRequest(email)) == null;
}