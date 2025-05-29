namespace Sefer.Backend.Api.Data.Models.Courses;

/// <summary>
/// This class implement some shared methods for content blocks
/// </summary>
internal static class Element
{
    /// <summary>
    /// Get a format text for a header (combination of number and heading)
    /// </summary>
    /// <remarks>We can argue if this should be refactored in a view</remarks>
    public static string GetHeaderText(IElement element)
    {
        var text = element.Number;
        if (string.IsNullOrEmpty(text) == false) { text += ": "; }
        return text + element.Heading;

    }
}