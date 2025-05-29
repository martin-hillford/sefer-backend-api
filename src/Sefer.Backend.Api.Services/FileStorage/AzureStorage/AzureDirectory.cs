namespace Sefer.Backend.Api.Services.FileStorage.AzureStorage;

/// <summary>
/// A directory in azure (although in blob storage directory don't really exists)
/// </summary>
public class AzureDirectory : AbstractDirectory<AzureDirectory>, IDirectory
{
    /// <summary>
    /// Holds a reference to the azure storage service
    /// </summary>
    private readonly AzureStorageService _azureStorageService;

    /// <summary>
    /// Simply defines of the directory separator in azure
    /// </summary>
    public const string AzureDirectorySeparator = @"/";

    /// <summary>
    /// Defines the name of the meta info file
    /// </summary>
    public const string DirectoryMetaInfo = ".dir.meta";

    /// <summary>
    /// Gets the name of the file
    /// </summary>
    public override string Name => WebUtility.UrlDecode(_name);
    private readonly string _name;

    /// <summary>
    /// Gets if this directory is a root directory
    /// </summary>
    private bool IsRoot => 
        string.IsNullOrEmpty(RelativeAddress) || 
        (RelativeAddress.Length == 1 && RelativeAddress[..1] == AzureDirectorySeparator);

    /// <summary>
    /// Gets the full path of this directory
    /// </summary>
    public override string Path
    {
        get
        {
            var relative = RelativeAddress;
            if (relative.StartsWith(AzureDirectorySeparator)) relative = relative.Substring(1);

            if (IsPublic) { return FileStorageServiceDefines.PublicPath + AzureDirectorySeparator + relative; }
            return FileStorageServiceDefines.PrivatePath + AzureDirectorySeparator + relative;
        }
    }

    /// <summary>
    /// Returns a list with all the directories within this directory
    /// </summary>
    public async Task<List<IDirectory>> GetDirectoriesAsync()
    {
        var container = _azureStorageService.GetContainerClient(IsPublic);
        var prefix = IsRoot ? RelativeAddress : RelativeAddress + AzureDirectorySeparator;
        var blobs = container.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, AzureDirectorySeparator, prefix);

        var directories = new List<IDirectory>();
        await foreach (var blob in blobs)
        {
            if (blob.IsBlob) continue;
            var dirPrefix = string.IsNullOrEmpty(RelativeAddress) ? blob.Prefix : blob.Prefix.Replace(RelativeAddress, string.Empty);
            var name = dirPrefix.Replace(AzureDirectorySeparator, string.Empty);
            var directory = new AzureDirectory(_azureStorageService, RelativeAddress + AzureDirectorySeparator + name, IsPublic);
            directories.Add(directory);
        }
        return directories.OrderBy(d => d.Name).ToList();
    }

    /// <summary>
    /// Returns a list with all the files within this directory
    /// </summary>
    public async Task<List<IFile>> GetFilesAsync()
    {
        var container = _azureStorageService.GetContainerClient(IsPublic);
        var prefix = IsRoot ? string.Empty : RelativeAddress + AzureDirectorySeparator;
        var blobs = container.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, AzureDirectorySeparator, prefix);
        var parent = IsRoot ? new AzureDirectory(_azureStorageService, RelativeAddress, IsPublic) : this;

        var files = new List<IFile>();
        await foreach (var blob in blobs)
        {
            if (!blob.IsBlob || blob.Blob.Name.EndsWith(DirectoryMetaInfo)) continue;
            var blobClient = _azureStorageService.GetBlobClient(blob.Blob, IsPublic);
            var file = new AzureFile(blobClient, parent, _azureStorageService);
            files.Add(file);
        }
        return files;
    }

    /// <summary>
    /// Creates a 'root' directory, providing a container
    /// </summary>
    /// <param name="azureStorageService"></param>
    /// <param name="relativeAddress"></param>
    /// <param name="isPublic"></param>
    public AzureDirectory(AzureStorageService azureStorageService, string relativeAddress, bool isPublic) : base(isPublic, relativeAddress)
    {
        var name = string.Empty;
        var parts = relativeAddress.Split(AzureDirectorySeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 0) name = parts[^1];
        _name = name;

        if (RelativeAddress.StartsWith(AzureDirectorySeparator)) RelativeAddress = relativeAddress.Substring(1);
        _azureStorageService = azureStorageService;
    }

    /// <summary>
    /// Deletes the directory
    /// </summary>
    /// <returns>True when the file is deleted, else false</returns>
    public async Task<bool> DeleteAsync()
    {
        try
        {
            var container = _azureStorageService.GetContainerClient(IsPublic);
            var prefix = RelativeAddress + AzureDirectorySeparator;
            var blobs = container.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix);

            await foreach (var blob in blobs)
            {
                await container.DeleteBlobIfExistsAsync(blob.Name);
            }

            return true;
        }
        catch (Exception) { return false; }
    }

    /// <summary>
    /// This method checks if provided name is file in this directory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<IFile> FindFileAsync(string name)
    {
        var absoluteName = RelativeAddress + AzureDirectorySeparator + name;
        var blob = _azureStorageService.GetBlobClient(absoluteName, IsPublic);

        if (blob == null || !blob.Exists()) return null;
        var file = new AzureFile(blob, this, _azureStorageService);
        return Task.FromResult<IFile>(file);
    }

    /// <summary>
    /// This method checks if provided name is file in this directory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private AzureDirectory FindDirectory(string name)
    {
        var absoluteName = RelativeAddress + AzureDirectorySeparator + name + DirectoryMetaInfo;
        var blob = _azureStorageService.GetBlobClient(absoluteName, IsPublic);
        if (blob == null || !blob.Exists()) return null;
        var directory = new AzureDirectory(_azureStorageService, RelativeAddress + AzureDirectorySeparator + name, IsPublic);
        return directory;
    }

    /// <summary>
    /// This method checks if provided name is file in this directory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<IDirectory> IDirectory.FindDirectoryAsync(string name)
    {
        var result = FindDirectory(name);
        return Task.FromResult<IDirectory>(result);
    }

    /// <summary>
    /// This method appends a new file to this directory
    /// </summary>
    /// <param name="fileName">The name to use for the file in the directory</param>
    /// <param name="readStream">A stream from which the bytes of the file can be read</param>
    /// <param name="overwrite">when set to true if the file already exists it will be overwritten</param>
    /// <param name="contentType"></param>
    public override async Task<IFile> AppendAsync(string fileName, Stream readStream, bool overwrite, string contentType)
    {
        try
        {
            var absoluteName = RelativeAddress + AzureDirectorySeparator + fileName;
            var blob = _azureStorageService.GetBlobClient(absoluteName, IsPublic);
            if (blob == null) return null;

            if (overwrite) await blob.DeleteIfExistsAsync();
            await blob.UploadAsync(readStream, new BlobHttpHeaders { ContentType = contentType });

            return new AzureFile(blob, this, _azureStorageService);
        }
        catch (Exception) { return null; }
    }

    /// <summary>
    /// Add a subdirectory to the current directory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<IDirectory> AddDirectoryAsync(string name)
    {
        try
        {
            var dirPath = IsRoot ? name : RelativeAddress + AzureDirectorySeparator + name;

            var absoluteName = dirPath + AzureDirectorySeparator + DirectoryMetaInfo;
            var blob = _azureStorageService.GetBlobClient(absoluteName, IsPublic);
            if (blob == null) return null;

            if (await blob.ExistsAsync()) return new AzureDirectory(_azureStorageService, dirPath, IsPublic);

            var bytes = Encoding.UTF8.GetBytes(DirectoryMetaInfo);
            var content = new MemoryStream(bytes);
            await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = "text/pain" });

            return new AzureDirectory(_azureStorageService, dirPath, IsPublic);
        }
        catch (Exception) { return null; }
    }
}