namespace Sefer.Backend.Api.Services.FileStorage.FileSystemStorage;

/// <summary>
/// The FileStorageService
/// </summary>
/// <inheritdoc cref="SharedStorageService{T}"/>
public class FileSystemStorageService : SharedStorageService<FileSystemStorageService>
{
    private readonly FileSystemDirectory _private;

    private readonly FileSystemDirectory _public;

    private readonly FileSystemStorageServiceOptions _options;

    /// <summary>
    /// Creates a new storage service capable of storing files on the file system
    /// </summary>
    public FileSystemStorageService(IOptions<FileSystemStorageServiceOptions> options)
    {
        _options = options.Value;
        _private = new FileSystemDirectory(options.Value.PrivatePath, false, options.Value);
        _public = new FileSystemDirectory(options.Value.PublicPath, true, options.Value);
    }

    /// <summary>
    /// A method that is capable of resolving a list of names as being subdirectories
    /// </summary>
    private IDirectory ResolveDirectoryPath(bool isPublic, List<string> names)
    {
        if (isPublic) return _public.FindDirectory(names);
        return _private.FindDirectory(names);
    }

    /// <summary>
    /// Resolves a file given a path (or identifier)
    /// </summary>
    /// <returns>null if a file was not found else the file itself</returns>
    /// <inheritdoc />
    public override async Task<IFile> ResolveFileAsync(string relativeAddress, bool isPublic)
    {
        // First create the directory path and the file name
        if (string.IsNullOrEmpty(relativeAddress)) { return null; }

        var directories = relativeAddress.Split(FileStorageServiceDefines.PathSeparator);
        var file = directories.Last();
        var path = string.Join(FileStorageServiceDefines.PathSeparator, directories.SkipLast(1));

        // Use the directory resolver to find the dirPath that should contain the file
        var directory = await ResolveDirectoryAsync(path, isPublic);
        if(directory == null) return null;
        return await directory.FindFileAsync(file);
    }

    /// <summary>
    /// Resolves a directory given a path (or identifier)
    /// </summary>
    /// <param name="relativeAddress">The directory to resolve</param>
    /// <param name="isPublic">when false, path will be resolved relative to the PrivatePath else the PublicPath is used</param>
    /// <returns>null if a file was not found else the file itself</returns>
    /// <inheritdoc />
    public override async Task<IDirectory> ResolveDirectoryAsync(string relativeAddress, bool isPublic)
    {
        var root = isPublic == false ? FileStorageServiceDefines.PrivatePath : FileStorageServiceDefines.PublicPath;
        return await ResolveDirectoryAsync(root + FileStorageServiceDefines.PathSeparator + relativeAddress);
    }

    /// <summary>
    /// Resolves a directory given a path (or identifier)
    /// </summary>
    /// <returns>null if a file was not found else the file itself</returns>
    private new Task<IDirectory> ResolveDirectoryAsync(string path) => Task.FromResult(ResolveDirectory(path));

    /// <summary>
    /// Resolves a directory given a path (or identifier)
    /// </summary>
    /// <returns>null if a file was not found else the file itself</returns>
    private IDirectory ResolveDirectory(string path)
    {
        // Check for root paths
        if (string.IsNullOrEmpty(path)) return null;
        if (path.StartsWith(FileStorageServiceDefines.PathSeparator)) path = path[1..];
        if (path.StartsWith(FileStorageServiceDefines.PublicPath) == false && path.StartsWith(FileStorageServiceDefines.PrivatePath) == false) { return null; }

        // Clear up double separator
        var doubleSep = FileStorageServiceDefines.PathSeparator + "" + FileStorageServiceDefines.PathSeparator;
        while (path.Contains(doubleSep)) { path = path.Replace(doubleSep, FileStorageServiceDefines.PathSeparator); }
        if (path.EndsWith(FileStorageServiceDefines.PathSeparator)) { path = path[..^1]; }

        if (path == FileStorageServiceDefines.PublicPath) return _public;
        if (path == FileStorageServiceDefines.PrivatePath) return _private;

        var names = path.Split([FileStorageServiceDefines.PathSeparator], StringSplitOptions.RemoveEmptyEntries).ToList();

        var root = names[0];
        names.RemoveAt(0);
        if (root == FileStorageServiceDefines.PublicPath) { return ResolveDirectoryPath(true, names); }
        if (root == FileStorageServiceDefines.PrivatePath) { return ResolveDirectoryPath(false, names); }
        return null;
    }

    /// <summary>
    /// This method parses a path and return a relative address and is the path is public
    /// </summary>
    public override (string RelativeAddress, bool IsPublic) ParsePath(string path)
    {
        // First ensure we replace the / with the proper system path
        path = path.Replace("/", FileStorageServiceDefines.PathSeparator);
        if (path.StartsWith("/")) path = path[1..];

        // If it is the actual path return make the addresses relative
        if (path == "public") return (string.Empty, true);
        if (path == "private") return (string.Empty, false);

        // Sub paths
        if (path.StartsWith("public")) return (path[7..], true);
        if (path.StartsWith("private")) return (path[8..], false);

        // No path at all
        throw new ArgumentException("Illegal file path", nameof(path));
    }

    private string GetUrl(string path, bool throwException)
    {
        try
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            if (path.StartsWith("/")) path = path[1..];
            if (path.StartsWith("public") || path.StartsWith("private")) 
                return _options.Endpoint + "/content/download/" + path;
            throw new ArgumentException("Illegal file path", nameof(path));
        }
        catch (Exception)
        {
            if (throwException) throw;
            return string.Empty;
        }
    }

    public override string GetUrl(string path) => GetUrl(path, false);
}