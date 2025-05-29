namespace Sefer.Backend.Api.Services.Mail.Abstractions;

/// <summary>
/// Simple structure defining e-mail addresses and name to display
/// </summary>
public struct MailAddress
{
    /// <summary>
    /// Create a new MailAddress
    /// </summary>
    /// <param name="email"></param>
    /// <param name="name"></param>
    public MailAddress(string email, string name)
    {
        Name = name;
        Email = email;
    }

    /// <summary>
    /// An e-mail address
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// The name to use when sending e-mail to the address
    /// </summary>
    public string Name { get; private set; }
}