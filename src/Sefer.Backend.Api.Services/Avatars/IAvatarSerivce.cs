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
    /// This generates the right hash given a userId. This is done to prevent enumeration of the avatar based
    /// on the userId.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public string GetAvatarId(int userId);
}