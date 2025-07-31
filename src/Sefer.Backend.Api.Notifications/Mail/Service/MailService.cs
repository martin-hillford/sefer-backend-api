namespace Sefer.Backend.Api.Notifications.Mail.Service;

public class MailService(IServiceProvider serviceProvider) : IMailService
{
    private readonly IMailServiceBase _baseMailService = serviceProvider.GetService<IMailServiceBase>();

    private readonly ICryptographyService _cryptographyService = serviceProvider.GetService<ICryptographyService>();

    private readonly MailServiceOptions _mailServiceOptions = serviceProvider.GetService<IOptions<MailServiceOptions>>()?.Value;

    private readonly ILogger<MailService> _logger = serviceProvider.GetService<ILogger<MailService>>();
    
    public async Task SendCompleteRegistrationEmailAsync(MailData data)
    {
        var model = new RegistrationCompleteModel(data);
        await SendMessageAsync("registration", model, data.Receiver);
    }

    public async Task SendCompleteInAppRegistrationEmailAsync(MailData data)
    {
        var model = new RegistrationCompleteModel(data);
        await SendMessageAsync("in_app_registration", model, data.Receiver);
    }

    public async Task SendPasswordForgotEmailAsync(MailData data)
    {
        var model = new PasswordForgotModel(data);
        await SendMessageAsync("password_forgot", model, data.Receiver);
    }

    public async Task SendStudentIsInactiveEmailAsync(MailData data)
    {
        var model = new UserMailModel(data);
        await SendMessageAsync("inactive_student", model, data.Receiver);
    }

    public async Task SendPasswordResetCompletedEmailAsync(MailData data)
    {
        var model = new PasswordResetModel(data);
        await SendMessageAsync("password_reset", model, data.Receiver);
    }

    public async Task SendAccountDeleteConfirmationEmailAsync(MailData data)
    {
        var model = new AccountDeleteModel(data);
        await SendMessageAsync("account_delete_request", model, data.Receiver);
    }

    public async Task SendEmailUpdateRequestedToOldAddressAsync(MailData data)
    {
        var model = new UserMailModel(data);
        await SendMessageAsync("notify_update_email_request", model, data.Receiver);
    }

    public async Task SendEmailUpdateRequestedToNewAddressAsync(MailData data, string newMail)
    {
        var encrypted = _cryptographyService.Encrypt(data.Receiver.Id + "-" + newMail);
        var model = new UpdateEmailRequestModel(data) { EmailChangeData = encrypted, NewMail = newMail };
        await SendMessageAsync("update_email_request", model, data.Receiver);
    }

    public async Task SendTwoFactorAuthEnabledEmailAsync(MailData data)
    {
        var model = new UserMailModel(data);
        await SendMessageAsync("two_factor_auth_enabled", model, data.Receiver);
    }

    /// <summary>
    /// This method sends an e-mail to the user to notify it he has disabled two-factor auth
    /// </summary>
    public async Task SendTwoFactorAuthDisabledEmailAsync(MailData data)
    {
        var model = new UserMailModel(data);
        await SendMessageAsync("two_factor_auth_disabled", model, data.Receiver);
    }

    public async Task SendEmailUpdateCompleteToOldAddressAsync(MailData data, string newMail, string oldMail)
    {
        var model = new UpdateEmailCompletedModel(data) { NewEmail = newMail, OldEmail = oldMail };
        var address = new MailAddress(oldMail, data.Receiver.Name);
        await SendMessageAsync("notify_email_update_completed", model, address);
    }

    public async Task SendEmailUpdateCompleteToNewAddressAsync(MailData data, string newMail, string oldMail)
    {
        var model = new UpdateEmailCompletedModel(data) { NewEmail = newMail, OldEmail = oldMail };
        var address = new MailAddress(newMail, data.Receiver.Name);
        await SendMessageAsync("email_update_completed", model, address);
    }

    public async Task SendRewardReceivedEmailAsync(MailData data, List<RewardGrant> rewards)
    {
        var task = new List<Task>();
        foreach (var reward in rewards)
        {
            if (reward.Reward?.Type != RewardTypes.VoucherReward) continue;
            task.Add(SendVoucherRewardEmailAsync(data, reward));
        }
        await Task.WhenAll(task);
    }

    public async Task SendVoucherRewardEmailAsync(MailData data, RewardGrant reward)
    {
        var model = new VoucherRewardModel(data, reward);
        var task = new List<Task>
        {
                SendMessageAsync("voucher_reward_student_email", model, data.Receiver),
            SendMessageAsync("voucher_reward_admin_email", model, new MailAddress(_mailServiceOptions.AdminEmail, _mailServiceOptions.AdminEmail))
        };

        await Task.WhenAll(task);
    }

    public string SendTestEmail(ISite site)
    {
        try
        {
            var message = GetTestMessage(site);
            _baseMailService.SendEmailSynchronously(message);
            return string.Empty;
        }
        catch (Exception exp) { return exp.Message + "\n\n" + exp.StackTrace; }
    }

    public async Task SendNotificationEmailAsync(MailData data, List<Message> messages, List<LessonSubmission> submissions)
    {
        if (messages == null || submissions == null || data.Receiver == null) return;
        if (messages.Count == 0 && submissions.Count == 0) return;

        var model = new NotificationModel(data) { Messages = messages.Cast(), Submissions = submissions };
        await SendMessageAsync("notification", model, data.Receiver);
    }

    public async Task SendEnrollmentEmailAsync(MailData data, User enrolledStudent, Course course)
    {
        var model = new EnrollmentModel(data) { StudentName = enrolledStudent.Name, MentorName = data.Receiver.Name, CourseName = course.Name };
        await SendMessageAsync("enrollment", model, data.Receiver);
        
        // Also, the admin needs to receive a copy of this e-mail
        await SendMessageAsync("enrollment", model, new MailAddress(_mailServiceOptions.AdminEmail, data.Receiver.Name));
    }

    private MailMessage GetTestMessage(ISite site)
    {
        var message = new MailMessage
        {
            Html = $"<p>This is test e-mail send from the '{site.Hostname}' sefer web-client</p>",
            Subject = "Test e-mail from sefer",
            Text = $"This is test e-mail send from the '{site.Hostname}' sefer web-client",
            ViewIdentifier = "TestMessage",
            SenderEmail = site.SendEmail,
            SenderName = site.SendEmail

        };
        message.To.Add(new MailAddress(_mailServiceOptions.AdminEmail, "Admin"));
        return message;
    }

    private async Task SendMessageAsync<T>( string mailView, T model, UserView receiver) where T : MailModel
        => await SendMessageAsync(mailView, model, new MailAddress(receiver.Email, receiver.Name));

    private async Task SendMessageAsync<T>(string mailView, T model,  MailAddress address) where T : MailModel
    {
        try
        {
            // Please note: the mail service is a scope service
            using var scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
            var renderService = scope.ServiceProvider.GetService<IViewRenderService>();
            
            if (mailView == null || model == null || string.IsNullOrEmpty(address.Email)) return;
            var html = await renderService.RenderToStringAsync(mailView, model.Language, "html", model);
            var text = await renderService.RenderToStringAsync(mailView, model.Language, "text", model);
            
            var message = new MailMessage
            {
                Html = html.Content,
                Text = text.Content,
                Subject = html.Title?.Replace("@site", model.Site.Name),
                SenderEmail = model.Site.SendEmail,
                SenderName = model.Site.Name,
                ViewIdentifier = mailView
            };

            message.To.Add(address);
            _baseMailService.QueueEmailForSending(message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error Occurred while sending email for model: {MailView}", mailView);
        }
    }
}