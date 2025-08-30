namespace Sefer.Backend.Api.Notifications;

public class NotificationService(IServiceProvider serviceProvider) : INotificationService
{
    #region Properties

    private readonly IMediator _mediator = serviceProvider.GetService<IMediator>();

    private readonly IWebSocketProvider _webSocket = serviceProvider.GetService<IWebSocketProvider>();

    private readonly IFireBaseService _push = serviceProvider.GetService<IFireBaseService>();

    private readonly IMailService _email = serviceProvider.GetService<IMailService>();
    
    private readonly ILogger<NotificationService> _logger = serviceProvider.GetService<ILogger<NotificationService>>();

    #endregion

    #region INotificationService Methods

    public async Task SendProfileUpdatedNotificationAsync(int userId, string oldName)
    {
        var messages = await Send(new PostNameChangeChatMessageRequest(oldName, userId));
        await _mediator.Send(new LoadMessageReferencesRequest(messages));

        var views = messages.SelectMany(c => c.ChannelMessages).Select(m => new MessageView(m));
        var task = views.Select(v => _webSocket.SendMessage(v));
        await Task.WhenAll(task);
    }

    public async Task SendChatMessageSendNotificationAsync(ChannelMessage channelMessage, bool ignoreHasRead = false)
    {
        var view = new MessageView(channelMessage);
        await _webSocket.SendMessage(view, ignoreHasRead);

        if (channelMessage.ReceiverId == channelMessage.Message.SenderId) return;

        switch (channelMessage.Message.Type)
        {
            case MessageTypes.Text:
                await _push.SendChatTextMessageNotification(channelMessage.ReceiverId, channelMessage.Message.Sender.Name, channelMessage.Message.ContentString);
                break;
        }
    }

    public Task SendChatMessageIsReadNotificationAsync(int messageId, User user)
    {
        // Just send mark message through the web socket
        return _webSocket.MarkMessageRead(messageId, user);
    }

    public async Task<bool> SendLessonSubmittedNotificationAsync(int submissionId, User mentor, User student)
    {
        using (_logger.BeginScope(Guid.NewGuid()))
        {
            // The mentor needs to receive a message in his inbox
            var message = await Send(new PostSubmissionMessageRequest(submissionId));
            if (message == null || mentor == null || student == null) return false;

            // Post a message into the message chat of the mentor and student
            var mentorView = new MessageView(message, message.Channel.Type, mentor.Id);
            await _webSocket.SendMessage(mentorView);

            var studentView = new MessageView(message, message.Channel.Type, student.Id);
            await _webSocket.SendMessage(studentView);

            // Also send a push notification to the mentor
            await _push.SendLessonSubmittedNotificationToMentor(submissionId, mentor, student);

            return true;
        }
    }

    public async Task<bool> SendSubmissionReviewedNotificationAsync(LessonSubmission submission)
    {
        // Check input and load required data
        var messages = await Send(new PostReviewChatMessageRequest(submission.Id));
        if (messages == null || messages.Count == 0) return false;

        var enrollment = await Send(new GetEnrollmentByIdRequest(submission.EnrollmentId));
        if (enrollment == null) return false;

        var student = await Send(new GetUserByIdRequest(enrollment.StudentId));
        if (student == null) return false;

        // Post the messages through the web socket
        var views = messages.Select(message => new MessageView(message, message.Channel.Type, student.Id));
        foreach (var view in views) { await _webSocket.SendMessage(view); }

        // Notify the student via the push notifications
        await _push.SendLessonReviewedNotificationToStudent(submission.Id, student);
        return true;
    }

    public async Task SendStudentEnrolledInCourseNotificationAsync(Course course, Enrollment enrollment)
    {
        try
        {
            // Post a message through the web sockets
            var posted = await Send(new PostEnrollmentChatMessageRequest(enrollment.Id));
            if (posted == null) return;

            // Send the notification through the web socket
            foreach (var channelMessage in posted.ChannelMessages)
            {
                var view = new MessageView(channelMessage);
                await _webSocket.SendMessage(view, true);
            }

            // Send an e-mail notification (please note, also the admin is mailed)
            var (region, site) = await Send(new GetPrimaryRegionAndSiteRequest(enrollment.Mentor.Id));
            var language = enrollment.Mentor.GetPreferredInterfaceLanguage();
            var data = new MailData(serviceProvider, enrollment.Mentor, site, region, language);
            await _email.SendEnrollmentEmailAsync(data, enrollment.Student, course);

            // Send the notification as a push notification to the mentor
            await _push.SendStudentEnrolledNotificationToMentor(course, enrollment);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch (Exception) { }
    }

    public async Task SendStudentIsInactiveNotificationAsync()
    {
        var settings = await Send(new GetSettingsRequest());
        var firstReminderDate = DateTime.UtcNow.Date.AddDays(-1 * settings.StudentReminderDays);
        var lastReminderDate = DateTime.UtcNow.Date.AddDays(-2 * settings.StudentReminderDays);

        var firstReminderStudents = await Send(new GetInactiveStudentsRequest(firstReminderDate));
        var lastReminderStudents = await Send(new GetInactiveStudentsRequest(lastReminderDate));
        var inActiveReminderStudents = firstReminderStudents.Union(lastReminderStudents).ToList();

        //  Please note: use the notification service not just the email service
        var tasks = inActiveReminderStudents.Select(SendStudentIsInactiveNotificationAsync);
        await Task.WhenAll(tasks);
    }

    public async Task SendStudentIsInactiveNotificationAsync(User student)
    {
        var data = await GetMailData(student);
        await _email.SendStudentIsInactiveEmailAsync(data);
        await _push.SendStudentIsInactiveNotificationAsync(student);
    }

    public async Task SendCompleteRegistrationNotificationAsync(User user, string language)
    {
        var mailData = await GetMailData(user, language);
        await _email.SendCompleteRegistrationEmailAsync(mailData);
    }

    public async Task SendCompleteInAppRegistrationNotificationAsync(User user, string language)
    {
        var mailData = await GetMailData(user, language);
        await _email.SendCompleteInAppRegistrationEmailAsync(mailData);
    }

    public async Task SendPasswordForgotNotificationAsync(User user, string language)
    {
        var mailData = await GetMailData(user, language);
        await _email.SendPasswordForgotEmailAsync(mailData);
    }

    public async Task SendPasswordResetCompletedNotificationAsync(User user, string language)
    {
        var mailData = await GetMailData(user, language);
        await _email.SendPasswordResetCompletedEmailAsync(mailData);
    }

    public async Task SendEmailUpdateRequestedNotificationAsync(User user, string language, string newMail)
    {
        var mailData = await GetMailData(user, language);
        await _email.SendEmailUpdateRequestedToNewAddressAsync(mailData, newMail);
        await _email.SendEmailUpdateRequestedToOldAddressAsync(mailData);
    }

    public async Task SendEmailUpdateCompleteNotificationAsync(User user, string language, string newMail, string oldMail)
    {
        var mailData = await GetMailData(user, language);
        await _email.SendEmailUpdateCompleteToNewAddressAsync(mailData, newMail, oldMail);
        await _email.SendEmailUpdateCompleteToOldAddressAsync(mailData, newMail, oldMail);
    }

    public async Task SendAccountDeleteConfirmationNotificationAsync(User user, string language)
    {
        var mailData = await GetMailData(user, language);
        await _email.SendAccountDeleteConfirmationEmailAsync(mailData);
    }

    public async Task SendRewardReceivedNotificationAsync(User user, List<RewardGrant> rewards)
    {
        var mailData = await GetMailData(user);
        await _email.SendRewardReceivedEmailAsync(mailData, rewards);
    }

    public async Task SendTwoFactorAuthDisabledNotificationAsync(User user, string language)
    {
        var mailData = await GetMailData(user, language);
        await _email.SendTwoFactorAuthDisabledEmailAsync(mailData);
    }

    public async Task SendTwoFactorAuthEnabledNotificationAsync(User user, string language)
    {
        var mailData = await GetMailData(user, language);
        await _email.SendTwoFactorAuthEnabledEmailAsync(mailData);
    }

    public string SendTestNotification(ISite site) => _email.SendTestEmail(site);

    #endregion

    #region Private Methods

    private async Task<MailData> GetMailData(User user, string language = null)
    {
        var lang = !string.IsNullOrEmpty(language) ? language : user.GetPreferredInterfaceLanguage();
        var (region, site) = await Send(new GetPrimaryRegionAndSiteRequest(user.Id));
        return new MailData(serviceProvider, user, site, region, lang);
    }

    private Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
        => _mediator.Send(request, token);

    #endregion
}