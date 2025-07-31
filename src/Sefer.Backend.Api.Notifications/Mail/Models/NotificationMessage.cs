// This is the post model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
// ReSharper disable MemberCanBeProtected.Global UnusedAutoPropertyAccessor.Global
using System.Text.Json;
using Sefer.Backend.Api.Data;

namespace Sefer.Backend.Api.Notifications.Mail.Models;

public class NotificationMessage
{
    private readonly Message _message;

    public readonly object Content;

    public string SenderName => _message.Sender.Name;

    public int Id => _message.Id;

    public MessageTypes Type => _message.Type;

    public NotificationMessage(Message message)
    {
        _message = message;
        Content = GetContent();
    }

    private object GetContent()
    {
        return _message.Type switch
        {
            MessageTypes.NameChange or MessageTypes.Text => _message.ContentString,
            MessageTypes.StudentLessonSubmission => JsonSerializer.Deserialize<SubmissionView>(_message.ContentString, Options),
            MessageTypes.MentorLessonSubmissionReview => JsonSerializer.Deserialize<ReviewedSubmissionView>(_message.ContentString, Options),
            MessageTypes.MentorAnswerReview => JsonSerializer.Deserialize<ReviewedAnswerView>(_message.ContentString, Options)?.MentorReview,
            _ => null,
        };
    }

    private static readonly JsonSerializerOptions Options = DefaultJsonOptions.GetOptions();
}

public static class MessageExtensions
{
    public static List<NotificationMessage> Cast(this List<Message> messages) => messages
        .Select(m => new NotificationMessage(m))
        .Where(m => m.Content != null)
        .ToList();
}