namespace Sefer.Backend.Api.Services.FileStorage.Abstraction;

/// <summary>
/// IDirectory represent a file, (which is in the end nothing more that a collection of Bytes with a name)
/// </summary>
public interface IFile
{
    /// <summary>
    /// Gets the size, in bytes, of the current file.
    /// </summary>
    long Size { get; }

    /// <summary>
    /// Gets the string representing the extension part of the file
    /// </summary>
    string Extension { get; }

    /// <summary>
    /// Gets the name of the file
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the content type of file, to be used in the browser
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Returns a stream that can be used for reading the bytes of this file
    /// </summary>
    Stream GetReadStream();

    /// <summary>
    /// Deletes the file
    /// </summary>
    /// <returns>True when the file is deleted, else false</returns>
    Task<bool> DeleteAsync();

    /// <summary>
    /// Gets if this file is public available
    /// </summary>
    bool IsPublic { get; }

    /// <summary>
    /// Gets the full path of this file
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Gets a unique identifier for the file;
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    string Id { get; }

    /// <summary>
    /// The uri of the file (can be used download the file)
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Gets the DownloadActionResult for the file download
    /// </summary>
    IActionResult DownloadActionResult { get; }
}