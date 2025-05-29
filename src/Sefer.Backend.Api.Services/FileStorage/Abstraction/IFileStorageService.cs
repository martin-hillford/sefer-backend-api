// ReSharper disable UnusedMember.Global

#nullable enable
namespace Sefer.Backend.Api.Services.FileStorage.Abstraction;

/// <summary>
/// This interface abstracts a service that is able to save and retrieve files
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// This method get a stream for reading and writes it to temporary storage.
    /// A unique identifier is returned switch later can be used for retrieving
    /// the temporary file.
    /// </summary>
    /// <param name="readStream">A stream from which the bytes of the file can be read</param>
    /// <returns></returns>
    /// <remarks>Storing any details about the temp file is NOT the responsibility of the storage service</remarks>
    string WriteTemp(Stream readStream);

    /// <summary>
    /// This method returns a stream that can be used for reading a file
    /// from the temporary storage given the unique identifier
    /// </summary>
    /// <param name="identifier">An identifier to retrieve the file for</param>
    Stream ReadTemp(string identifier);

    /// <summary>
    /// Move the temporary file with provided identifier to the directory saving it as filename
    /// </summary>
    /// <param name="identifier">The identifier of the temporary file</param>
    /// <param name="directory">The directory to save the file in</param>
    /// <param name="fileName">The name of the file to save</param>
    /// <param name="contentType">The mime type of the content</param>
    Task MoveTempAsync(string identifier, IDirectory directory, string fileName, string contentType);

    /// <summary>
    /// Deletes a temporary file
    /// </summary>
    /// <param name="identifier"></param>
    void DeleteTemp(string identifier);

    /// <summary>
    /// Returns a url that can be used to display the image in the browser
    /// </summary>
    /// <param name="path"></param>
    string GetUrl(string path);

    /// <summary>
    /// This method parses a path and return a relative address and is the path is public
    /// </summary>
    /// <param name="path">The path to parse</param>
    /// <returns>The relativeAddress and if that is in a public or private container</returns>
    (string RelativeAddress, bool IsPublic) ParsePath(string path);

    /// <summary>
    /// Resolves a file given a path
    /// </summary>
    /// <param name="path">The path to resolve</param>
    /// <returns>null if a file was not found else the file itself</returns>
    Task<IFile?> ResolveFileAsync(string path);

    /// <summary>
    /// Resolves a file given a relativeAddress
    /// </summary>
    /// <param name="relativeAddress">The relativeAddress to resolve</param>
    /// <param name="isPublic">when false, relativeAddress will be resolved relative to the PrivatePath else the PublicPath is used</param>
    /// <returns>null if a file was not found else the file itself</returns>
    Task<IFile> ResolveFileAsync(string relativeAddress, bool isPublic);

    /// <summary>
    /// Resolves a directory given a path
    /// </summary>
    /// <param name="path">The path to resolve</param>
    /// <returns>null if a directory was not found else directory file itself</returns>
    Task<IDirectory> ResolveDirectoryAsync(string path);

    /// <summary>
    /// Resolves a directory given a relativeAddress
    /// </summary>
    /// <param name="relativeAddress">The directory to resolve</param>
    /// <param name="isPublic">when false, relativeAddress will be resolved relative to the PrivatePath else the PublicPath is used</param>
    /// <returns>null if a file was not found else the file itself</returns>
    Task<IDirectory> ResolveDirectoryAsync(string relativeAddress, bool isPublic);

    /// <summary>
    /// This method helps is stripped paths from file name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string StripPath(string name);
}