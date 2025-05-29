// ReSharper disable once UnusedMember.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Services.FileStorage.AbstractStorage;

public abstract class AbstractDirectory<T> where T : IDirectory
{
    private readonly T _baseParent;

    /// <summary>
    /// Holds the Parent if this directory (null when this is a root)
    /// </summary>
    public IDirectory Parent => _baseParent;
    
    /// <summary>
    /// Gets the name of the file
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets if files in this directory are public available
    /// </summary>
    public bool IsPublic { get; protected set; }

    /// <summary>
    /// Gets the full path of this file
    /// </summary>
    public abstract string Path { get; }

    /// <summary>
    /// Returns the relative address of the directory
    /// </summary>
    public string RelativeAddress { get; protected init; }

    /// <summary>
    /// Simply creator to set the fields
    /// </summary>
    protected AbstractDirectory(T parent, string relativeAddress)
    {
        IsPublic = parent.IsPublic;
        _baseParent = parent;
        RelativeAddress = relativeAddress;
    }

    /// <summary>
    /// Simply creator to set the fields
    /// </summary>
    /// <param name="isPublic"></param>
    /// <param name="relativeAddress"></param>
    protected AbstractDirectory(bool isPublic, string relativeAddress)
    {
        IsPublic = isPublic;
        RelativeAddress = relativeAddress;
    }

    /// <summary>
    /// Appends an IFormFile to this directory
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public async Task<IFile> AppendAsync(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        return await AppendAsync(SharedStorageService.CleanFileName(file.FileName), stream, true, file.ContentType);
    }

    /// <summary>
    /// This method appends a new file to this directory
    /// </summary>
    /// <param name="fileName">The name to use for the file in the directory</param>
    /// <param name="readStream">A stream from which the bytes of the file can be read</param>
    /// <param name="overwrite">when set to true if the file already exists it will be overwritten</param>
    /// <param name="contentType"></param>
    public abstract Task<IFile> AppendAsync(string fileName, Stream readStream, bool overwrite, string contentType);
}