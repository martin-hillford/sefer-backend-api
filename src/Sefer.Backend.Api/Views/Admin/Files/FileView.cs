// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Views.Admin.Files;

/// <summary>
/// A view on IFile
/// </summary>
public class FileView
{
    #region Protected

    /// <summary>
    /// The model the file is based upon
    /// </summary>
    protected readonly IFile Model;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the size, in bytes, of the current file.
    /// </summary>
    public long Size => Model.Size;

    /// <summary>
    /// Gets the string representing the extension part of the file
    /// </summary>
    public string Extension => Model.Extension;

    /// <summary>
    /// Gets the name of the file
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// Gets the content type of file, to be used in the browser
    /// </summary>
    public string ContentType => Model.ContentType;

    /// <summary>
    /// Gets the full path of this file
    /// </summary>
    public string Path => Model.Path;

    /// <summary>
    /// The uri of the file (can be used download the file)
    /// </summary>
    public string Url => Model.Url;

    #endregion

    #region Properties

    /// <summary>
    /// Create a view on a file
    /// </summary>
    /// <param name="model"></param>
    public FileView(IFile model)
    {
        Model = model;
    }

    #endregion
}