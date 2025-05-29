// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Services.FileStorage.Abstraction;

/// <summary>
/// IDirectory represent a container for file storage
/// </summary>
public interface IDirectory
{
    /// <summary>
    /// Gets the name of the directory
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets if files in this directory are public available
    /// </summary>
    bool IsPublic { get; }

    /// <summary>
    /// Gets the full path of this file
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Returns the relative address of the directory
    /// </summary>
    string RelativeAddress { get; }

    /// <summary>
    /// Holds the Parent if this directory (null when this is a root)
    /// </summary>
    IDirectory Parent { get; }

    /// <summary>
    /// Returns a list with all the directories within this directory
    /// </summary>
    Task<List<IDirectory>> GetDirectoriesAsync();

    /// <summary>
    /// Returns a list with all the files within this directory
    /// </summary>
    Task<List<IFile>> GetFilesAsync();

    /// <summary>
    /// This method checks if provided name is file in this directory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<IFile> FindFileAsync(string name);

    /// <summary>
    /// This method checks if provided name is subdirectory in this directory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<IDirectory> FindDirectoryAsync(string name);

    /// <summary>
    /// Appends an IFormFile to this directory
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    Task<IFile> AppendAsync(IFormFile file);

    /// <summary>
    /// This method appends a new file to this directory
    /// </summary>
    /// <param name="fileName">The name to use for the file in the directory</param>
    /// <param name="readStream">A stream from which the bytes of the file can be read</param>
    /// <param name="overwrite">when set to true if the file already exists it will be overwritten</param>
    /// <param name="contentType">The mime type of the content</param>
    Task<IFile> AppendAsync(string fileName, Stream readStream, bool overwrite, string contentType);

    /// <summary>
    /// Deletes the directory
    /// </summary>
    /// <returns>True when the directory (and all it subdirectories and file) is deleted, else false</returns>
    Task<bool> DeleteAsync();

    /// <summary>
    /// Creates a subdirectory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<IDirectory> AddDirectoryAsync(string name);
}