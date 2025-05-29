namespace Sefer.Backend.Api.Views.Admin.Files;

/// <summary>
/// A view on an IDirectory
/// </summary>
public class DirectoryView
{
    /// <summary>
    /// Holds the model directory
    /// </summary>
    protected readonly IDirectory Model;

    /// <summary>
    /// Gets the name of the directory
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// Gets the full path of this file
    /// </summary>
    public string Path => Model.Path;

    /// <summary>
    /// A list of directories within this directory
    /// </summary>
    public List<DirectoryView> Directories { get; private set; } = new List<DirectoryView>();

    /// <summary>
    /// A list of files within this directory
    /// </summary>
    public List<FileView> Files { get; private set; } = new List<FileView>();

    /// <summary>
    /// Create a new directory view
    /// </summary>
    /// <param name="directory"></param>
    private DirectoryView(IDirectory directory)
    {
        Model = directory;
    }

    /// <summary>
    /// Creates a new directory
    /// </summary>
    /// <param name="model">The model directory this view is based upon</param>
    /// <param name="deepLoad">When set to true the full tree is loaded, else only directory sub children and files</param>
    /// <param name="includeFiles">When set to true files will be included</param>
    public static Task<DirectoryView> CreateViewAsync(IDirectory model, bool deepLoad, bool includeFiles)
        => CreateViewAsync(model, deepLoad, 0, includeFiles);

    /// <summary>
    /// Creates a directory
    /// </summary>
    /// <param name="directory">The model directory this view is based upon</param>
    /// <param name="deepLoad">When set to true the full tree is loaded, else it depends on the depth</param>
    /// <param name="depth">The depth of directory. When set to 0 the children will be loaded</param>
    /// <param name="includeFiles">When set to true files will be included</param>
    protected static async Task<DirectoryView> CreateViewAsync(IDirectory directory, bool deepLoad, int depth, bool includeFiles)
    {
        var view = new DirectoryView(directory);

        if (deepLoad || depth == 0)
        {
            var subDirectories = await directory.GetDirectoriesAsync();
            var tasks = subDirectories.Select(d => CreateViewAsync(d, deepLoad, depth + 1, includeFiles));
            view.Directories = (await Task.WhenAll(tasks)).ToList();

            if (includeFiles)
            {
                var files = await directory.GetFilesAsync();
                view.Files = files.Select(f => new FileView(f)).ToList();
            }
        }

        return view;
    }

}
