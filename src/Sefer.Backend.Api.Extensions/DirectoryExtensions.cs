// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Extensions;

/// <summary>
/// Extension methods on directories and other files system related methods
/// </summary>
public static class DirectoryExtensions
{
    /// <summary>
    /// This method will return if this directory has any files or directory
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static bool IsEmpty(this DirectoryInfo directory)
    {
        return directory.EnumerateFiles().Any() ||
               directory.EnumerateDirectories().Any();
    }

    /// <summary>
    /// This method will empty a given directory from any files and directories
    /// </summary>
    /// <param name="directory"></param>
    public static void Empty(this DirectoryInfo directory)
    {
        if(directory.IsEmpty()) return;
        foreach(var file in directory.GetFiles()) file.Delete();
        foreach(var subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
    }
}