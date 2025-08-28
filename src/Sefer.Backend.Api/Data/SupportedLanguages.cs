using System.Collections.Immutable;

namespace Sefer.Backend.Api.Data;

/// <summary>
/// This class contains all the support languages by the api
/// </summary>
public static class SupportedLanguages
{
    private const string DefaultLanguage = "nl";

    private static readonly ImmutableHashSet<string> Languages = ImmutableHashSet.Create("nl","en");

    public static string GetLanguage(string language)
    {
        var lang = language.ToLower();
        return Languages.Contains(lang) ? lang : DefaultLanguage;
    }

    // ReSharper disable once UnusedMember.Global
    public static bool IsDefaultLanguage(string language) => 
        DefaultLanguage.Equals(language, StringComparison.CurrentCultureIgnoreCase);
}
