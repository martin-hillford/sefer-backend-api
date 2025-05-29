namespace Sefer.Backend.Api.Support.Security;

/// <summary>
/// A service that deals with passwords
/// </summary>
public sealed class PasswordService : IPasswordService
{
    /// <summary>
    /// The service which will helps with the cryptography
    /// </summary>
    private readonly ICryptographyService _cryptographyService;

    /// <summary>
    /// The options to use when encrypting
    /// </summary>
    private readonly SecurityOptions _options;

    /// <summary>
    /// Creates a new PasswordService
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cryptographyService"></param>
    public PasswordService(IOptions<SecurityOptions> options, ICryptographyService cryptographyService)
    {
        _options = options.Value;
        _cryptographyService = cryptographyService;
    }

    /// <summary>
    /// This method updates the password of the given user
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    public void UpdatePassword(IUser user, string password)
    {
        user.PasswordSalt = _cryptographyService.Salt();
        user.Password = GeneratePassword(password, user.PasswordSalt);
    }

    /// <summary>
    /// This method validates if the given password is equal to the users
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool IsValidPassword(IUser user, string password)
    {
        var generate = GeneratePassword(password, user.PasswordSalt);
        return generate == user.Password;
    }

    /// <summary>
    /// Generate a password hash
    /// </summary>
    /// <param name="password"></param>
    /// <param name="salt"></param>
    /// <returns></returns>
    private string GeneratePassword(string password, string salt)
    {
        // convert the salt to bytes, get the data protection salt, join them and create a new salt
        var saltBytes = Convert.FromBase64String(salt);
        var dataBytes = Convert.FromBase64String(_options.DataProtectionKey);
        var bytes = new byte[saltBytes.Length + dataBytes.Length];
        saltBytes.CopyTo(bytes, 0);
        dataBytes.CopyTo(bytes, saltBytes.Length);
        var passwordSalt = Convert.ToBase64String(bytes);

        // We have now an application, user specific password, use the cryptography service to create a hash
        // Todo: when upgrading for the shared crypto lib ensure this will be HashPassword!
        return _cryptographyService.Hash(password, passwordSalt);
    }
}