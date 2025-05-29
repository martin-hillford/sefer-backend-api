namespace Sefer.Backend.Api.Services.Avatars;

public class AvatarService(IOptions<AvatarOptions> options) : IAvatarService
{
    private readonly AvatarOptions _options = options.Value;

    /// <summary>
    /// Generate an avatar url for the given user
    /// </summary>
    public string GetAvatarUrl(int userId, string userName)
    {
        var hash = GetAvatarId(userId);
        var initials = GetInitials(userName);

        return $"{_options.Service}/avatar?hash={hash}&initials={initials}";
    }

    /// <summary>
    /// Generate an avatar url for the given user
    /// </summary>
    public string GetNonCachedAvatarUrl(int userId, string userName)
    {
        var hash = GetAvatarId(userId);
        var initials = GetInitials(userName);

        return $"{_options.Service}/avatar-no-cache?hash={hash}&initials={initials}";
    }

    /// <summary>
    /// The avatar service is capable of retrieving also avatars from gravatars
    /// This method will generate the correct url for this.
    /// </summary>
    public string GetGravatarUrl(string userEmail)
    {
        var hash = GravatarHash(userEmail);
        return $"{_options.Service}/gravatar?hash={hash}";
    }

    private static string GetInitials(string userName)
    {
        var value = userName?.Trim().ToUpper();
        if (string.IsNullOrEmpty(value)) return string.Empty;

        var parts = value.Split(' ');
        var first = parts[0][0].ToString();

        if (parts.Length == 1) return first;
        return first + parts[^1][0];
    }

    private static string GravatarHash(string email)
    {
        var bytes = Encoding.ASCII.GetBytes(email.Trim().ToLower());
        var hash = MD5.HashData(bytes);
        return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
    }

    private string GetAvatarId(int userId)
    {
        var bytes = Encoding.ASCII.GetBytes($"{userId}_{_options.ApiKey}");
        var hash = SHA512.HashData(bytes);
        return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
    }

    public string GetAvatarUploadUrl(int userId)
    {
        var hash = GetAvatarId(userId);
        return $"{_options.Service}/upload?hash={hash}";
    }
}