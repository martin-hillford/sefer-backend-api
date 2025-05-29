namespace Sefer.Backend.Api.Services.FileStorage.FileSystemStorage;

/// <summary>
/// A file already exception is used thrown when a file already exists and cannot be overridden
/// </summary>
/// <remarks>
/// Create a file already exception
/// </remarks>
/// <param name="name">The name of the file</param>
[Serializable]
public class FileAlreadyExistsException(string name) : Exception($"Could not open file {name}") { }