namespace Sefer.Backend.Api.Services.Avatars;

public interface IAvatarService
{
    /// <summary>
    /// Generate an avatar url for the given user
    /// </summary>
    public string GetAvatarUrl(int userId, string userName);

    /// <summary>
    /// Generate an avatar url for the given user
    /// </summary>
    public string GetNonCachedAvatarUrl(int userId, string userName);

    /// <summary>
    /// The avatar service is capable of retrieving also avatars from gravatars
    /// This method will generate the correct url for this.
    /// </summary>
    public string GetGravatarUrl(string userEmail);

    /// <summary>
    /// Generates the url that can be used to upload an avatar of a user
    /// </summary>
    public string GetAvatarUploadUrl(int userId);
}