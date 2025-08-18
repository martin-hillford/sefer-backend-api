using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Controllers.Abstract;

public abstract class ProfileController(IServiceProvider serviceProvider) : UserController(serviceProvider)
{
    private readonly IPasswordService _passwordService = serviceProvider.GetService<IPasswordService>();

    protected readonly INotificationService NotificationService = serviceProvider.GetService<INotificationService>();

    protected async Task<ActionResult> UpdateProfileInformation(IHttpContextAccessor httpContextAccessor, int userId)
    {
        // Verify that a user is provided
        var user = await Send(new GetUserByIdRequest(userId));
        if (user == null) return Forbid();
        
        // Because we have to support both the old and the new api scheme regarding user settings use the http context
        var settings = await Send(new GetUserSettingsRequest(user.Id));
        var profile = await UserSettingsHelper.FromJson<ProfileInfoPostModel>(httpContextAccessor, settings);
        if(profile == null || settings == null) return BadRequest();

        // check if the password the user has provided is valid (403)
        var validPassword = _passwordService.IsValidPassword(user, profile.Password);
        if (validPassword == false) return Forbid();

        // Check if the changes are valid and if the email address is unique (400)
        if (ModelState.IsValid == false) return BadRequest();
        if (!await IsUserEmailUnique(profile.Email, user.Id)) return BadRequest();

        // Now update the profile (but not the email address of the user) (500)
        var oldName = user.Name;
        user.Name = profile.Name;
        user.Gender = profile.Gender;
        user.YearOfBirth = profile.YearOfBirth;
        user.Info = profile.Info;
        user.AdditionalInfo = profile.AdditionalInfo;

        var valid = await Send(new UpdateUserRequest(user));
        if (valid == false) return StatusCode(500);

        // If there is a change in the e-mail of the user, send a change request to the user
        var statusCode = 200;
        var newEmail = profile.Email.ToLower().Trim();
        if (newEmail != user.Email.ToLower().Trim())
        {
            await NotificationService.SendEmailUpdateRequestedNotificationAsync(user, profile.Language, newEmail);
            statusCode = 202;
        }

        // Notify the user of the user has changed his name
        // Thread safety is really an issues in this flow
        if (oldName.ToLower().Trim() != user.Name.ToLower().Trim())
        {
            await NotificationService.SendProfileUpdatedNotificationAsync(userId, oldName);
        }
        
        // Save the additional user settings to the database
        var settingsSaved = await Send(new UpdateUserSettingsRequest(user.Id, settings));
        if(!settingsSaved) return StatusCode(500);

        // Everything is done! (202 for email change 202 for regular)
        var view = new UserView(user);
        return Json(view, statusCode);
    }

    private async Task<bool> IsUserEmailUnique(string email, int userId)
    {
        var user = await Send(new GetUserByEmailRequest(email));
        return user == null || user.Id == userId;
    }
}