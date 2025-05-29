namespace Sefer.Backend.Api.Services.FileStorage.AbstractStorage;

/// <summary>
/// An abstract that deals with the storage of temporal files
/// </summary>
// ReSharper disable once UnusedTypeParameter
// NB. The generic type is needed for extending purposes
public abstract class TempStorageProvider<T>
{
    #region Properties

    /// <summary>
    /// A store for the tempDir, to be created once at starting the service for the first time
    /// </summary>
    private readonly string _tempDir;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new TempStorageProvider
    /// </summary>
    protected TempStorageProvider()
    {
        _tempDir = FileUtils.GetTemporaryDirectory();
        while (File.Exists(_tempDir)) { _tempDir = FileUtils.GetTemporaryDirectory(); }
        Directory.CreateDirectory(_tempDir);
    }

    #endregion

    #region Methods

    /// <summary>
    /// This method get a stream for reading and writes it to temporary storage.
    /// A unique identifier is returned switch later can be used for retrieving
    /// the temporary file.
    /// </summary>
    /// <param name="readStream">A stream from which the bytes of the file can be read</param>
    /// <returns></returns>
    /// <remarks>Storing any details about the temp file is NOT the responsibility of the storage service</remarks>
    public string WriteTemp(Stream readStream)
    {
        var identifier = Path.GetRandomFileName();
        var writer = new FileStream(Path.Combine(_tempDir, identifier), FileMode.CreateNew);
        readStream.CopyTo(writer);
        return identifier;
    }

    /// <summary>
    /// This method returns a stream that can be used for reading a file
    /// from the temporary storage given the unique identifier
    /// </summary>
    /// <param name="identifier">An identifier to retrieve the file for</param>
    /// <returns>If identifier is not found null is returned else a stream from which can be read</returns>
    public Stream ReadTemp(string identifier)
    {
        var file = Path.Combine(_tempDir, identifier);
        if (File.Exists(file) == false) { return null; }
        var reader = new FileStream(file, FileMode.Open, FileAccess.Read);
        return reader;
    }

    /// <summary>
    /// Deletes a temporary file
    /// </summary>
    /// <param name="identifier"></param>
    public void DeleteTemp(string identifier)
    {
        var file = Path.Combine(_tempDir, identifier);
        if (File.Exists(file)) File.Delete(file);
    }

    /// <summary>
    /// Move the temporary file with provided identifier to the directory saving it as filename
    /// </summary>
    /// <param name="identifier">The identifier of the temporary file</param>
    /// <param name="directory">The directory to save the file in</param>
    /// <param name="fileName">The name of the file to save</param>
    /// <param name="contentType"></param>
    public async Task MoveTempAsync(string identifier, IDirectory directory, string fileName, string contentType)
    {
        var file = Path.Combine(_tempDir, identifier);
        if (File.Exists(file) == false) return;

        await directory.AppendAsync(fileName, ReadTemp(identifier), false, contentType);
        DeleteTemp(identifier);
    }

    #endregion
}