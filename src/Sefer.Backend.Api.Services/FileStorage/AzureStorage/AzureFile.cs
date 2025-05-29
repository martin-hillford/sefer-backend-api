namespace Sefer.Backend.Api.Services.FileStorage.AzureStorage;

/// <summary>
/// Represent a wrapper around blob stored in azure
/// </summary>
/// <inheritdoc cref="IFile"/>
/// <inheritdoc cref="AbstractFile"/>
public class AzureFile : AbstractFile, IFile
{
    #region Properties

    /// <summary>
    /// A store for the service
    /// </summary>
    private readonly AzureStorageService _azureStorageService;

    /// <summary>
    /// Holds the path of the file on disk
    /// </summary>
    private readonly BlobClient _blob;

    private readonly BlobProperties _blobProperties;

    /// <summary>
    /// Gets the size, in bytes, of the current file.
    /// </summary>
    /// <inheritdoc />
    public long Size => _blobProperties.ContentLength;

    /// <summary>
    /// Gets the string representing the extension part of the file
    /// </summary>
    /// <inheritdoc />
    public string Extension
    {
        get
        {
            var name = _blob.Name;
            if (name.Contains('.') == false) { return string.Empty; }
            return name[(name.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase) + 1)..];
        }
    }

    /// <summary>
    /// Gets the name of the file
    /// </summary>
    /// <inheritdoc cref="AbstractFile.Name" />
    public override string Name => 
        _blob.Name[(_blob.Name.LastIndexOf(AzureDirectory.AzureDirectorySeparator, StringComparison.Ordinal) + 1)..];

    /// <summary>
    /// The uri of the file (can be used download the file)
    /// </summary>
    /// <inheritdoc />
    public string Url => _azureStorageService.GetUrl(Path);

    /// <summary>
    /// Gets the content type of file, to be used in the browser
    /// </summary>
    /// <inheritdoc />
    public string ContentType => _blobProperties.ContentType;

    /// <summary>
    /// Gets a unique identifier for the file;
    /// </summary>
    /// <inheritdoc />
    public string Id => Path;

    /// <summary>
    /// Returns a stream that can be used for reading the bytes of this file
    /// </summary>
    /// <inheritdoc />
    public Stream GetReadStream()
    {
        try { return _blob.OpenReadAsync().Result; }
        catch (Exception) { throw new Exception($"Could not open file {Name}"); }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new file (reflection a file storage file)
    /// </summary>
    /// <param name="file"></param>
    /// <param name="parent"></param>
    /// <param name="azureStorageService"></param>
    /// <inheritdoc />
    internal AzureFile(BlobClient file, AzureDirectory parent, AzureStorageService azureStorageService) : base(parent)
    {
        _blob = file;
        _azureStorageService = azureStorageService;
        _blobProperties = _blob.GetProperties().Value;
    }

    /// <summary>
    /// Gets the DownloadActionResult for the file download
    /// </summary>
    /// <inheritdoc />
    public IActionResult DownloadActionResult
    {
        get
        {
            if (IsPublic) return new RedirectResult(_blob.Uri.AbsoluteUri);
            return new FileStreamResult(GetReadStream(), ContentType);
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes the file
    /// </summary>
    /// <returns>True when the file is deleted, else false</returns>
    /// <inheritdoc />
    public async Task<bool> DeleteAsync()
    {
        try { return await _blob.DeleteIfExistsAsync(); }
        catch (Exception) { return false; }
    }

    #endregion
}