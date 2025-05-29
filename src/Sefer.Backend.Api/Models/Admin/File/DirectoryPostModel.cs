// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.File;

/// <summary>
/// Defines the post for a directory creation
/// </summary>
public class DirectoryPostModel
{
    /// <summary>
    /// The name of the directory to create
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The parent directory to create the parent in
    /// </summary>
    public string Path { get; set; }
}
