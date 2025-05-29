namespace Sefer.Backend.Api.Services.Mail.Abstractions;

/// <summary>
/// MailAddressList holds a list of e-mail address and names, to be used to send mail to
/// </summary>
public class MailAddressList
{
    #region Properties

    /// <summary>
    /// A store for saving the saving addresses
    /// </summary>
    private readonly Dictionary<string, MailAddress> _address;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new empty MailAddressList
    /// </summary>
    public MailAddressList() { _address = []; }

    #endregion

    #region Methods

    /// <summary>
    /// Adds an email address and name to the list
    /// </summary>
    /// <param name="email">The e-mail address to add</param>
    /// <param name="name">The name of the </param>
    /// <returns>True when the email address is in the list else false</returns>
    private bool Add(string email, string name)
    {
        // If the email address is already in the list, update the name and return true
        if (_address.ContainsKey(email))
        {
            _address[email] = new MailAddress(email, name);
            return true;
        }

        // if it's not a valid address false should be return
        var regex = new RegexUtilities();
        if (regex.IsValidEmail(email) == false) return false;

        _address.Add(email, new MailAddress(email, name));
        return true;

    }

    /// <summary>
    /// Adds an email address to the list
    /// </summary>
    /// <param name="address">The e-mail address to add</param>
    /// <returns>True when the email address is in the list else false</returns>
    public bool Add(MailAddress address) { return Add(address.Email, address.Name); }

    /// <summary>
    /// Returns a list of all addresses in the list
    /// </summary>
    /// <returns>Returns a list of all addresses in the list</returns>
    public IReadOnlyList<MailAddress> GetAddresses()
    {
        return _address.Values.ToList().AsReadOnly();
    }

    #endregion
}
