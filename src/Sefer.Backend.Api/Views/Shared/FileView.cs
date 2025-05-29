// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Views.Shared;

/// <summary>
/// A view on an uploaded file
/// </summary>
public class FileView
{
    /// <summary>
    /// Contains the uri of the file that base been uploaded;
    /// </summary>
    public readonly string Url;

    /// <summary>
    /// Creates a new file view
    /// </summary>
    /// <param name="file"></param>
    public FileView(IFile file)
    {
        Url = file.Url;
    }
}
