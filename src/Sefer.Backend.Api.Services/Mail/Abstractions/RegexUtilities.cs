using System.Globalization;

namespace Sefer.Backend.Api.Services.Mail.Abstractions;

/// <summary>
/// A class providing some regular expression utilities
/// </summary>
public class RegexUtilities
{
    /// <summary>
    /// Returns if the given string mail is a valid e-mail address
    /// </summary>
    /// <param name="mail">The string to test</param>
    /// <returns>true if a valid e-mail address</returns>
    /// <see href="https://msdn.microsoft.com/en-us/library/01escwtf(v=vs.110).aspx"/>
    public bool IsValidEmail(string mail)
    {
        // If the string is empty, return false
        if (string.IsNullOrEmpty(mail)) { return false; }

        // Use IdnMapping class to convert Unicode domain names.
        try
        {
            mail = Regex.Replace(mail, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
        }
        catch (Exception) { return false; }


        // Return true if mail is in valid e-mail format.
        try
        {
            return Regex.IsMatch(mail,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException) { return false; }
    }

    /// <summary>
    /// The domain name along with the @ character is passed to the DomainMapper method, which uses the IdnMapping
    /// class to translate Unicode characters that are outside the US-ASCII character range to Puny code.
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    private string DomainMapper(Match match)
    {
        // IdnMapping class with default property values.
        var idn = new IdnMapping();
        var domainName = idn.GetAscii(match.Groups[2].Value);
        return match.Groups[1].Value + domainName;
    }
}
