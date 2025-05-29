// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Services.FileStorage.FileSystemStorage;

/// <summary>
/// The FileStorageDirectory implements IDirectory using an actual directory on the file system
/// </summary>
public class FileSystemDirectory : AbstractDirectory<FileSystemDirectory>, IDirectory
{
    /// <summary>
    /// Holds the path of the directory on disk
    /// </summary>
    private readonly DirectoryInfo _info;

    /// <summary>
    /// Holds the options of the service (required to make the url)
    /// </summary>
    private readonly FileSystemStorageServiceOptions _options;

    /// <summary>
    /// Gets the full path of this directory
    /// </summary>
    public override string Path
    {
        get
        {
            if (Parent != null) return Parent.Path + System.IO.Path.DirectorySeparatorChar + _info.Name;
            var basePath = "private";
            if (IsPublic) basePath = "public";
            return System.IO.Path.DirectorySeparatorChar + basePath;
        }
    }

    /// <summary>
    /// Create a new directory 'root' storage directory, it reflects either the public or the private directory
    /// </summary>
    /// <param name="fileSystemDirectory"></param>
    /// <param name="isPublic"></param>
    /// <param name="options"></param>
    internal FileSystemDirectory(string fileSystemDirectory, bool isPublic, FileSystemStorageServiceOptions options) : base(isPublic, string.Empty)
    {
        _info = new DirectoryInfo(fileSystemDirectory);
        _options = options;
    }

    /// <summary>
    /// Create a new directory (reflection a file storage directory)
    /// </summary>
    /// <param name="fileSystemDirectory"></param>
    /// <param name="parent"></param>
    /// <param name="relativeAddress"></param>
    /// <param name="options"></param>
    private FileSystemDirectory(string fileSystemDirectory, FileSystemDirectory parent, string relativeAddress, FileSystemStorageServiceOptions options) : base(parent, relativeAddress)
    {
        _info = new DirectoryInfo(fileSystemDirectory);
        _options = options;
    }

    /// <summary>
    /// Gets the name of the file
    /// </summary>
    public override string Name => _info.Name;

    /// <summary>
    /// Returns a list with all the directories within this directory
    /// </summary>
    public Task<List<IDirectory>> GetDirectoriesAsync()
    {
        var directories = new List<IDirectory>();
        foreach (string directory in Directory.EnumerateDirectories(_info.FullName))
        {
            var dir = new DirectoryInfo(directory);
            string relativeAddress = RelativeAddress + System.IO.Path.DirectorySeparatorChar + dir.Name;
            directories.Add(new FileSystemDirectory(directory, this, relativeAddress, _options));
        }
        var result = directories.OrderBy(d => d.Name).ToList();
        return Task.FromResult(result);
    }

    /// <summary>
    /// Returns a list with all the files within this directory
    /// </summary>
    public Task<List<IFile>> GetFilesAsync()
    {
        var files = new List<IFile>();
        foreach (string file in Directory.EnumerateFiles(_info.FullName))
        {
            files.Add(new FileSystemFile(_options, file, this));
        }
        return Task.FromResult(files);
    }

    /// <summary>
    /// Returns a list with files in this directory matching the given pattern
    /// </summary>
    /// <param name="pattern">The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.</param>
    /// <returns>An enumerable collection of the files in the directory specified by path and that match the specified search pattern.</returns>
    public List<IFile> FindFiles(string pattern)
    {
        return Directory
            .EnumerateFiles(_info.FullName, pattern)
            .Select(file => new FileSystemFile(_options, file, this))
            .Cast<IFile>()
            .ToList();
    }

    /// <summary>
    /// This method appends a new file to this directory
    /// </summary>
    /// <param name="fileName">The name to use for the file in the directory</param>
    /// <param name="readStream">A stream from which the bytes of the file can be read</param>
    /// <param name="overwrite">when set to true if the file already exists it will be overwritten</param>
    /// <param name="contentType">The content type (will be ignored here, the file system has its own content type methods)</param>
    public override async Task<IFile> AppendAsync(string fileName, Stream readStream, bool overwrite, string contentType)
    {
        var fullName = _info.FullName + System.IO.Path.DirectorySeparatorChar + fileName;
        if (File.Exists(fileName) && overwrite == false) { throw new FileAlreadyExistsException(fileName); }

        await using (var writeStream = new FileStream(fullName, FileMode.OpenOrCreate))
        {
            await readStream.CopyToAsync(writeStream);
        }
        return new FileSystemFile(_options, fileName, this);
    }
    /// <summary>
    /// Deletes the directory
    /// </summary>
    /// <returns>True when the directory (and all it subdirectories and file) is deleted, else false</returns>
    public Task<bool> DeleteAsync()
    {
        var deleted = Delete();
        return Task.FromResult(deleted);
    }

    /// <summary>
    /// Deletes the directory
    /// </summary>
    /// <returns>True when the directory (and all it subdirectories and file) is deleted, else false</returns>
    private bool Delete()
    {
        try { Directory.Delete(_info.FullName, true); return true; } catch (Exception) { return false; }
    }

    /// <summary>
    /// This method checks if provided name is subdirectory in this directory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<IDirectory> FindDirectoryAsync(string name) => Task.FromResult(FindDirectory(name));

    /// <summary>
    /// This method checks if provided name is subdirectory in this directory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private IDirectory FindDirectory(string name)
    {
        var dirPath = _info.FullName + System.IO.Path.DirectorySeparatorChar + name;
        if (Directory.Exists(dirPath) == false) { return null; }
        var relativeAddress = RelativeAddress + System.IO.Path.DirectorySeparatorChar + name;
        return new FileSystemDirectory(dirPath, this, relativeAddress, _options);
    }

    /// <summary>
    /// This method recursively resolves directories with the provided list of names
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    internal IDirectory FindDirectory(List<string> names)
    {
        // Check if there is anything to resolve
        if (names == null) { return null; }
        if (names.Any() == false) { return null; }

        // Get the current name to check
        var current = names[0];
        var sub = FindDirectory(current) as FileSystemDirectory;
        if (sub == null) { return null; }

        // Check if recursive resolving is required
        names.RemoveAt(0);
        if (names.Any() == false) { return sub; }
        return sub.FindDirectory(names);
    }

    /// <summary>
    /// Add a subdirectory to the current directory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<IDirectory> AddDirectoryAsync(string name) => Task.FromResult(AddDirectory(name));

    /// <summary>
    /// Add a subdirectory to the current directory
    /// </summary>
    /// <param name="name"></param>
    private IDirectory AddDirectory(string name)
    {
        try
        {
            var dirPath = _info.FullName + System.IO.Path.DirectorySeparatorChar + name;
            if (Directory.Exists(dirPath)) return null;
            Directory.CreateDirectory(dirPath);
            var relativeAddress = RelativeAddress + System.IO.Path.DirectorySeparatorChar + name;
            return new FileSystemDirectory(dirPath, this, relativeAddress, _options);
        }
        catch (Exception) { return null; }
    }

    /// <summary>
    /// This method checks if provided name is file in this directory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<IFile> FindFileAsync(string name) => Task.FromResult(FindFile(name));

    /// <summary>
    /// This method checks if provided name is file in this directory
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private IFile FindFile(string name)
    {
        var dirPath = _info.FullName + System.IO.Path.DirectorySeparatorChar + name;
        return File.Exists(dirPath) == false ? null : new FileSystemFile(_options, dirPath, this);
    }
}