namespace Sefer.Backend.Api.Services.FileStorage.AbstractStorage;

/// <summary>
/// An abstracts class that implements shared method with the file and azure service
/// </summary>
public static class SharedStorageService
{
    /// <summary>
    /// This method helps in providing clean stripped filenames
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string CleanFileName(string name)
    {
        name = StripPath(name, '/');
        name = StripPath(name, '\\');
        var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        var regexp = new Regex($"[{Regex.Escape(regexSearch)}]");
        return CleanUrl(regexp.Replace(name, ""));
    }

    public static string CleanUrl(string path)
    {
        var url = WebUtility.UrlDecode(path);
        var regexPath = new Regex(@"[^a-zA-Z0-9_\-/\\.]");
        var clean = regexPath.Replace(url, "");
        return clean;
    }

    /// <summary>
    /// Strips paths form a filename using the provide separator
    /// </summary>
    /// <param name="name"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    private static string StripPath(string name, char separator)
    {
        if (name.Contains(separator) == false) { return name; }
        var dubSep = separator + string.Empty + separator;
        while (name.Contains(dubSep)) { name = name.Replace(dubSep, separator.ToString()); }
        return name.Substring(name.LastIndexOf(separator) + 1);
    }
}

/// <summary>
/// A generic class that implements shared method with the file and azure service
/// </summary>
/// <typeparam name="T"></typeparam>
/// <inheritdoc cref="TempStorageProvider{T}"/>
public abstract class SharedStorageService<T> : TempStorageProvider<T>, IFileStorageService
{
    /// <inheritdoc />
    protected SharedStorageService() { }

    /// <inheritdoc />
    public string StripPath(string name)
    {
        return SharedStorageService.CleanFileName(name);
    }

    /// <summary>
    /// Resolves a file given a path
    /// </summary>
    /// <param name="path">The path to resolve</param>
    /// <returns>null if a file was not found else the file itself</returns>
    public async Task<IFile> ResolveFileAsync(string path)
    {
        try
        {
            var (relativeAddress, isPublic) = ParsePath(path);
            return await ResolveFileAsync(relativeAddress, isPublic);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Resolves a directory given a path
    /// </summary>
    /// <param name="path">The directory to resolve</param>
    /// <returns>null if a directory was not found else the file itself</returns>
    public async Task<IDirectory> ResolveDirectoryAsync(string path)
    {
        try
        {
            var (relativeAddress, isPublic) = ParsePath(path);
            return await ResolveDirectoryAsync(relativeAddress, isPublic);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    /// <inheritdoc />
    public abstract Task<IFile> ResolveFileAsync(string relativeAddress, bool isPublic);

    /// <inheritdoc />
    public abstract Task<IDirectory> ResolveDirectoryAsync(string relativeAddress, bool isPublic);

    /// <inheritdoc />
    public abstract string GetUrl(string path);

    /// <inheritdoc />
    public abstract (string RelativeAddress, bool IsPublic) ParsePath(string path);
}