// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public.Users;

/// <summary>
/// This model defines all the required input for deleting the account
/// </summary>
/// <inheritdoc />
public class RemoveAccountPostModel : ITimeProtectedModel
{
    /// <summary>
    /// This method contains the Data with encrypted information to deal with the change of the email
    /// </summary>
    /// <inheritdoc />
    public string Data => User;

    /// <summary>
    /// This method contains the Data with encrypted information to deal with the change of the email
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Random stuff acting as a second salt
    /// </summary>
    /// <inheritdoc />
    public string Random { get; set; }

    /// <summary>
    /// The date the url was generated
    /// </summary>
    /// <inheritdoc />
    public string Date { get; set; }

    /// <summary>
    /// The hash to prove the url is generated by the back-end
    /// </summary>
    /// <inheritdoc />
    public string Hash { get; set; }
}

