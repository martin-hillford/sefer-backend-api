namespace Sefer.Backend.Api.Services.FileStorage.AzureStorage;

/// <summary>
/// This is storage for files using azure blob storage
/// </summary>
/// <remarks>
/// Creates a new storage service capable of storing files on the file system
/// </remarks>
/// <param name="options"></param>
public class AzureStorageService(IOptions<AzureStorageServiceOptions> options) : SharedStorageService<AzureStorageService>
{
    /// <summary>
    /// A store for the options
    /// </summary>
    private readonly AzureStorageServiceOptions _options = options.Value;

    /// <summary>
    /// This method parses a path and return a relative address and is the path is public
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public override (string RelativeAddress, bool IsPublic) ParsePath(string path)
    {
        if (path.StartsWith(AzureDirectory.AzureDirectorySeparator)) path = path.Substring(1);
        if (path.StartsWith(FileStorageServiceDefines.PublicPath))
        {
            var address = path[FileStorageServiceDefines.PublicPath.Length..];
            if (address.StartsWith(AzureDirectory.AzureDirectorySeparator)) address = address.Substring(1);
            return (address, true);
        }
        if (path.StartsWith(FileStorageServiceDefines.PrivatePath))
        {
            var address = path[FileStorageServiceDefines.PrivatePath.Length..];
            if (address.StartsWith(AzureDirectory.AzureDirectorySeparator)) address = address.Substring(1);
            return (address, false);
        }
        throw new FileNotFoundException();
    }

    /// <summary>
    /// This method returns the contains where the information is stored in
    /// </summary>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    internal BlobContainerClient GetContainerClient(bool isPublic)
    {
        // Retrieve storage account from connection string.
        var blobSasUrl = isPublic ? _options.PublicSas : _options.PrivateSas;
        return new BlobContainerClient(new Uri(blobSasUrl));
    }

    /// <summary>
    /// Resolves a directory given a path (or identifier)
    /// </summary>
    /// <param name="relativeAddress">The directory to resolve</param>
    /// <param name="isPublic">true is we have to resolve a public file else false</param>
    /// <returns>null if a file was not found else the file itself</returns>
    public override async Task<IDirectory> ResolveDirectoryAsync(string relativeAddress, bool isPublic)
    {
        // ensure the path start with a slash
        if (string.IsNullOrEmpty(relativeAddress)) return new AzureDirectory(this, string.Empty, isPublic);
        if (relativeAddress.EndsWith(AzureDirectory.AzureDirectorySeparator)) relativeAddress = relativeAddress[..^1];
        if (relativeAddress.StartsWith(AzureDirectory.AzureDirectorySeparator)) relativeAddress = relativeAddress[1..];

        var metaPath = relativeAddress + AzureDirectory.AzureDirectorySeparator + AzureDirectory.DirectoryMetaInfo;
        var file = await ResolveFileAsync(metaPath, isPublic);
        if (file == null) return null;

        return new AzureDirectory(this, relativeAddress, isPublic);
    }

    /// <summary>
    /// Resolves a file given a path (or identifier)
    /// </summary>
    /// <param name="relativeAddress">The relativeAddress to resolve</param>
    /// <param name="isPublic"></param>
    /// <returns>null if a file was not found else the file itself</returns>
    public override async Task<IFile> ResolveFileAsync(string relativeAddress, bool isPublic)
    {
        // ensure the path start with a slash
        if (relativeAddress.StartsWith(AzureDirectory.AzureDirectorySeparator)) relativeAddress = relativeAddress.Substring(1);

        // get the container and check if a reference is found
        var container = GetContainerClient(isPublic);
        var reference = container.GetBlobClient(relativeAddress);
        if (reference == null || await reference.ExistsAsync() == false) return null;

        // create the azure file
        var dirRelative = relativeAddress;
        if (dirRelative.Contains(AzureDirectory.AzureDirectorySeparator))
        {
            dirRelative = dirRelative[..relativeAddress
                .LastIndexOf(AzureDirectory.AzureDirectorySeparator, StringComparison.Ordinal)];
        }
        var directory = new AzureDirectory(this, dirRelative, isPublic);
        return new AzureFile(reference, directory, this);
    }

    /// <summary>
    /// Returns a url that can be used to display the image in the browser
    /// </summary>
    /// <param name="path"></param>
    public override string GetUrl(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            var (relativeAddress, isPublic) = ParsePath(path);
            var baseUrl = _options.PrivateUrl;
            if (isPublic) baseUrl = _options.PublicUrl;
            if (baseUrl.EndsWith(AzureDirectory.AzureDirectorySeparator)) baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
            if (relativeAddress.StartsWith(AzureDirectory.AzureDirectorySeparator)) relativeAddress = relativeAddress.Substring(1);
            if (baseUrl.EndsWith(AzureDirectory.AzureDirectorySeparator)) return baseUrl + relativeAddress;
            var url = baseUrl + AzureDirectory.AzureDirectorySeparator + relativeAddress;
            return url;
        }
        catch (Exception) { return string.Empty; }
    }

    internal BlobClient GetBlobClient(BlobItem blob, bool isPublic)
    {
        var container = GetContainerClient(isPublic);
        return container.GetBlobClient(blob.Name);
    }

    internal BlobClient GetBlobClient(string name, bool isPublic)
    {
        try
        {
            var container = GetContainerClient(isPublic);
            return container.GetBlobClient(name);
        }
        catch (Exception) { return null; }
    }
}