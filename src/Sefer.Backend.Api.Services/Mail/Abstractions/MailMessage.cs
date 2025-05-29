// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Services.Mail.Abstractions;

/// <summary>
/// Defines a MailMessage. Please note, the FROM address is omitted. Since many mail services do
/// require configuration where the sender address is apart from, the from address should be handled
/// with in the mail service itself
/// </summary>
public class MailMessage
{
    /// <summary>
    /// The Subject of the E-mail
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// The e-mail address of the sender
    /// </summary>
    public string SenderEmail { get; set; }

    /// <summary>
    /// The name of sender
    /// </summary>
    public string SenderName { get; set; }

    /// <summary>
    /// A list of mail addresses to which the message should be sent
    /// </summary>
    public readonly MailAddressList To;

    /// <summary>
    /// The ViewModel to use for rendering the view with the content
    /// </summary>
    public string Html { get; set; }

    /// <summary>
    /// The type of body to be rendered (default = Both)
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Contains the (optional) ViewIdentifier for the html / text that is sent
    /// </summary>
    public string ViewIdentifier { get; set; }

    /// <summary>
    /// Creates a new MailMessage
    /// </summary>
    public MailMessage()
    {
        To = new MailAddressList();
    }
}
