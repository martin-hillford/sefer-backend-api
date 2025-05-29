namespace Sefer.Backend.Api.Services.FileStorage.Abstraction;

/// <summary>
/// Some general defines that can be used by file storage services
/// </summary>
public static class FileStorageServiceDefines
{
    /// <summary>
    /// The constant for the root of the public paths
    /// </summary>
    public const string PublicPath = "public";

    /// <summary>
    /// The constant for the root of the private paths
    /// </summary>
    public const string PrivatePath = "private";

    /// <summary>
    /// This is what separate paths, not on the file system, but own convention
    /// </summary>
    public const string PathSeparator = @"/";
}