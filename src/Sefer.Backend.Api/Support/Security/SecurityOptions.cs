// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global 
// ReSharper disable MemberCanBePrivate.Global, UnusedMember.Global
using Sefer.Backend.Authentication.Lib.Cryptography;

namespace Sefer.Backend.Api.Support.Security;

/// <summary>
/// A security provider using the json configuration
/// </summary>
public class SecurityOptions : ISecurityOptions
{
    /// <summary>
    /// Default data value
    /// </summary>
    private const string DefaultDataKey = "3yqpg0dg6pirwLqT0ap4CSfSys7cjTxGF+JT37asfVAQQhFr9DObz/lVBrl8n4dA08jB1wIgJiDtodXA6AfJrw==";

    /// <summary>
    /// Internal field
    /// </summary>
    private string _dataKey;

    /// <summary>
    /// A key used for protecting data saved in some location
    /// </summary>
    public string DataProtectionKey
    {
        get => string.IsNullOrEmpty(_dataKey) ? DefaultDataKey : _dataKey;
        set => _dataKey = value;
    }

    /// <summary>
    /// Default data value
    /// </summary>
    private const string DefaultUrlKey = "IYplMHzryOf+cWX+k8MiXWnPgBgOcTmfzspbt0l8pyMkJXZSRbYvxElJ3Ay++5lN6nZfwP0R66Q5mSrqgTsZXQ==";

    /// <summary>
    /// Internal field
    /// </summary>
    private string _urlKey;

    /// <summary>
    /// A key used for protecting data saved in some location
    /// </summary>
    public string UrlProtectionKey
    {
        get => string.IsNullOrEmpty(_urlKey) ? DefaultUrlKey : _dataKey;
        set => _urlKey = value;
    }
    
    /// <summary>
    /// The number of hours a token is valid
    /// </summary>
    public string TokenDuration { get; set; }

    /// <summary>
    /// The number of hours a token is valid
    /// </summary>
    public int TokenDurationInt
    {
        get
        {
            if (TokenDuration == null) return 0;
            return int.Parse(TokenDuration);
        }
        set => TokenDuration = value.ToString();
    }

    /// <summary>
    /// The path to set in the cookie
    /// </summary>
    public string CookiePath { get; set; }

    /// <summary>
    /// Only allow the cookies when this is set to true
    /// </summary>
    public string UseFileServiceCookie { get; set; }

    /// <summary>
    /// Only allow the cookies when this is set to true
    /// </summary>
    public bool UseFileServiceCookieBool
    {
        get => UseFileServiceCookie != "false";
        set => UseFileServiceCookie = (value) ? "true" : "false";
    }

    /// <summary>
    /// Force cookies over https
    /// </summary>
    public string CookieSecure { get; set; }

    /// <summary>
    /// Force cookies over https
    /// </summary>
    public bool CookieSecureBool
    {
        get => CookieSecure != "false";
        set => CookieSecure = value ? "true" : "false";
    }

    /// <summary>
    /// Implementation for ISecurityOptions
    /// </summary>
    public bool SecureCookie => CookieSecureBool;
}
