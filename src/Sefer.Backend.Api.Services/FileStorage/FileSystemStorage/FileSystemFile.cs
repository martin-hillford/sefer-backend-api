namespace Sefer.Backend.Api.Services.FileStorage.FileSystemStorage;

/// <summary>
/// The FileSystemFile implements IFile using an actual file on the file system
/// </summary>
/// <inheritdoc cref="IFile" />
/// <inheritdoc cref="AbstractFile" />
public class FileSystemFile : AbstractFile, IFile
{
    #region Files

    /// <summary>
    /// Holds the path of the file on disk
    /// </summary>

    private readonly FileInfo _info;

    /// <summary>
    /// Holds the options of the service (required to make the url)
    /// </summary>
    private readonly FileSystemStorageServiceOptions _options;

    /// <summary>
    /// The uri of the file (can be used download the file)
    /// </summary>
    /// <inheritdoc />
    public string Url
    {
        get
        {
            string path = Path.Replace(FileStorageServiceDefines.PathSeparator, "/");
            return $"{_options.Endpoint}/content/download{path}";
        }
    }

    /// <summary>
    /// Gets the size, in bytes, of the current file.
    /// </summary>
    /// <inheritdoc />
    public long Size => _info.Length;

    /// <summary>
    /// Gets the string representing the extension part of the file
    /// </summary>
    /// <inheritdoc />
    public string Extension => _info.Extension;

    /// <summary>
    /// Gets the name of the file
    /// </summary>
    /// <inheritdoc cref="AbstractFile.Name" />
    public override string Name => _info.Name;

    /// <summary>
    /// Gets the content type of file, to be used in the browser
    /// </summary>
    /// <inheritdoc />
    public string ContentType
    {
        get
        {
            new FileExtensionContentTypeProvider().TryGetContentType(Path, out var contentType);
            return contentType ?? "application/octet-stream";
        }
    }

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
        try
        {
            return File.Open(_info.FullName, FileMode.Open, FileAccess.Read);
        }
        catch (Exception) { throw new FileAlreadyExistsException(Name); }
    }

    /// <summary>
    /// Gets the DownloadActionResult for the file download
    /// </summary>
    public IActionResult DownloadActionResult
    {
        get
        {
            var response = new PhysicalFileResult(_info.FullName, ContentType)
            {
                FileDownloadName = Name,
                LastModified = _info.LastWriteTimeUtc,
            };
            return response;
        }
    }
    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new file (reflection a file storage file)
    /// </summary>
    /// <param name="options"></param>
    /// <param name="fileSystemFile"></param>
    /// <param name="parent"></param>
    /// <inheritdoc />
    internal FileSystemFile(FileSystemStorageServiceOptions options, string fileSystemFile, IDirectory parent) : base(parent)
    {
        _info = new FileInfo(fileSystemFile);
        _options = options;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes the file
    /// </summary>
    /// <returns>True when the file is deleted, else false</returns>
    /// <inheritdoc />
    public Task<bool> DeleteAsync() => Task.FromResult(Delete());

    /// <summary>
    /// Deletes the file
    /// </summary>
    /// <returns>True when the file is deleted, else false</returns>
    private bool Delete()
    {
        try { File.Delete(_info.FullName); return true; } catch (Exception) { return false; }
    }

    #endregion
}