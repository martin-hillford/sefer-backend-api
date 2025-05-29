// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Services.Mail.Abstractions;

/// <summary>
/// Defines the options for the e-mail service to send e-mail
/// </summary>
public class MailServiceOptions
{
    /// <summary>
    /// The host to use when sending
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// The port to connect with the host to use when sending
    /// </summary>
    public string Port { get; set; }

    /// <summary>
    /// The port to connect with the host to use when sending
    /// </summary>
    public int PortInt
    {
        get => string.IsNullOrEmpty(Port) ? 0 : int.Parse(Port);
        set => Port = value.ToString();
    }

    /// <summary>
    /// Only set when explicit SSL must be used, TLS is auto
    /// </summary>
    /// <remarks>A string is used for proper parsing of environment variables</remarks>
    public string UseSsl { get; set; }

    /// <summary>
    /// Only set when explicit SSL must be used, TLS is auto
    /// </summary>
    /// <value></value>
    public bool UseSslBoolean => UseSsl != "false";

    /// <summary>
    /// The username to use to connect with the host
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The password to use to connect with the host
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Returns the e-mail address of the administrator to be used in communications
    /// </summary>
    public string AdminEmail { get; set; }
    
    /// <summary>
    /// Only when service is enabled, actually mail is send
    /// </summary>
    public bool Enabled { get; set;  }
    
    /// <summary>
    /// A location to write a copy to
    /// </summary>
    public string WriteCopy { get; set;  }
}
