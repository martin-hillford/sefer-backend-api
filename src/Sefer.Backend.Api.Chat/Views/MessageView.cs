using System.Text.Json;
using System.Text.Json.Serialization;
using Sefer.Backend.Api.Data;

namespace Sefer.Backend.Api.Chat.Views;

/// <summary>
/// A view for messages send between users
/// </summary>
/// <inheritdoc />
public class MessageView : AbstractView<Message>
{
    /// <summary>
    /// The Channel this message is send into
    /// </summary>
    public int ChannelId => Model.ChannelId;

    /// <summary>
    /// The id of the user that has sent the message
    /// </summary>
    public int SenderId => Model.SenderId;

    /// <summary>
    /// Holds the name of the sender
    /// </summary>
    public string SenderName => Model.Sender.Name;

    /// <summary>
    /// The time the sender has sent the message
    /// </summary>
    public DateTime SenderDate => Model.SenderDate;

    /// <summary>
    /// Holds if the user is sender of this message
    /// </summary>
    public readonly bool? IsSender;

    /// <summary>
    /// The id of message that is being quoted
    /// </summary>
    public int? QuotedMessageId => Model.QuotedMessageId;

    /// <summary>
    /// The text of the quoted message (stripped)
    /// </summary>
    public string QuotedMessage => Model.QuotedMessageString;

    /// <summary>
    /// (optional) The user that has sent the quoted message
    /// </summary>
    public string QuotedUser => Model?.QuotedMessage?.Sender?.Name;

    /// <summary>
    /// The Type of the message
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageTypes Type => Model.Type;

    /// <summary>
    /// Holds if the sender has deleted the message
    /// </summary>
    public bool IsDeleted => Model.IsDeleted;

    /// <summary>
    /// Holds a list of receivers of the message
    /// </summary>
    public List<ReceiverView> Receivers { get; init; } = [];

    /// <summary>
    /// Holds the content of the message
    /// </summary>
    public object Content { get; private set; }

    /// <summary>
    /// The id of the user for whom this view is created
    /// </summary>
    [JsonIgnore]
    public readonly int UserId;

    /// <summary>
    /// The send state of the message. Since the message is in the database that state is delivered
    /// </summary>
    public string SendState = "delivered";

    /// <summary>
    /// This creates a view for fully loaded channel message
    /// (Message, Message.Channel, Message.ChannelMessages)
    /// </summary>
    /// <param name="channelMessage"></param>
    public MessageView(ChannelMessage channelMessage)
        : this(channelMessage.Message, channelMessage.Message.Channel.Type, channelMessage.ReceiverId) { }

    /// <summary>
    /// Creates a new View
    /// </summary>
    public MessageView(Message message, ChannelTypes channelType, int userId) : base(message)
    {
        // Save if this is the sender of the e-mail
        UserId = userId;
        IsSender = UserId == message.SenderId;

        // Create the list of the receivers
        if (!message.IsDeleted && channelType != ChannelTypes.Public)
        {
            Receivers = message.ChannelMessages.Select(c => new ReceiverView(c)).ToList();
        }

        if (message.IsDeleted) return;

        // Let's leave the deserialization of the message to the client.
        Content = GetContent(message);
    }

    private static object GetContent(Message message)
    {
        return message.Type switch
        {
            MessageTypes.StudentLessonSubmission => JsonSerializer.Deserialize<SubmissionView>(message.ContentString, Options),
            MessageTypes.MentorLessonSubmissionReview => JsonSerializer.Deserialize<ReviewedSubmissionView>(message.ContentString, Options),
            MessageTypes.StudentEnrollment => JsonSerializer.Deserialize<EnrollmentView>(message.ContentString, Options),
            MessageTypes.MentorAnswerReview => JsonSerializer.Deserialize<ReviewedAnswerView>(message.ContentString, Options),
            _ => message.ContentString,
        };
    }

    private static readonly JsonSerializerOptions Options = DefaultJsonOptions.GetOptions();
}
