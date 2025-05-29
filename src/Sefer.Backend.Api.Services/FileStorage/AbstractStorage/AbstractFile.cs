// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Services.FileStorage.AbstractStorage;

/// <summary>
/// Defines some abstraction for file that are only depended on the interface (IFile)
/// </summary>
public abstract class AbstractFile
{
    /// <summary>
    /// Returns the Parent of this file
    /// </summary>
    public readonly IDirectory Parent;

    /// <summary>
    /// Gets if this files is public available
    /// </summary>
    public bool IsPublic => Parent.IsPublic;

    /// <summary>
    /// Gets the full path of this file
    /// </summary>
    public string Path
    {
        get
        {
            var parent = Parent.Path;
            var name = Name;
            if (parent.EndsWith(FileStorageServiceDefines.PathSeparator)) parent = parent[..^1];
            if (name.StartsWith(FileStorageServiceDefines.PathSeparator)) name = name[1..];
            return parent + FileStorageServiceDefines.PathSeparator + name;
        }
    }

    /// <summary>
    /// Gets the relative path of this file
    /// </summary>
    public string RelativePath => Parent.RelativeAddress + FileStorageServiceDefines.PathSeparator + Name;

    /// <summary>
    /// Gets the name of the file
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Creates an abstract file, providing the directory
    /// </summary>
    /// <param name="parent"></param>
    protected AbstractFile(IDirectory parent)
    {
        Parent = parent;
    }
}