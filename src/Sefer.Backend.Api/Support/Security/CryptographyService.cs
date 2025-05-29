using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Sefer.Backend.Api.Shared.Cryptography;

namespace Sefer.Backend.Api.Support.Security;

/// <summary>
/// The _cryptographyService
/// </summary>
/// <inheritdoc />
public sealed class CryptographyService : ICryptographyService
{
    #region Constants

    /// <summary>
    /// Defines a date time format to be used for timing
    /// </summary>
    private const string DateTimeFormat = "yyyyMMddHHmm";

    /// <summary>
    /// A key used for storing in the data protection provider
    /// </summary>
    private const string Key = "Sefer.Backend.Api";

    #endregion

    #region Properties

    private readonly IDataProtectionProvider _dataProtectionProvider;

    /// <summary>
    /// Holds the security options
    /// </summary>
    private readonly SecurityOptions _options;

    #endregion

    #region Constructor

    /// <summary>
    /// Create a new cryptography which helps with encryption
    /// </summary>
    /// <param name="dataProtectionProvider"></param>
    /// <param name="options"></param>
    public CryptographyService(IDataProtectionProvider dataProtectionProvider, IOptions<SecurityOptions> options)
    {
        _dataProtectionProvider = dataProtectionProvider;
        _options = options.Value;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns a hash from the given value (which will also be used as salt)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string Hash(string value)
    {
        return Hash(value, _options.DataProtectionKey);
    }

    /// <summary>
    /// Returns this the given hash is response hash for the given data
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <inheritdoc />
    public bool IsValidHash(string hash, string data)
    {
        return hash == Hash(data, _options.DataProtectionKey);
    }

    /// <summary>
    /// The string value to provide a salted hash for.
    /// </summary>
    /// <param name="value">The value to hash</param>
    /// <param name="salt">The salt to use</param>
    /// <returns>A strong (like SHA256 type) hash of value with a salt included</returns>
    /// <inheritdoc />
    public string Hash(string value, string salt) => Hashing.HashPassword(value, salt);

    /// <summary>
    /// Returns this the given hash is response hash for the given data
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="data"></param>
    /// <param name="salt"></param>
    /// <returns>True if it is a valid hash, else false</returns>
    /// <inheritdoc />
    public bool IsValidHash(string hash, string data, string salt)
    {
        return hash == Hash(data, salt);
    }

    /// <summary>
    /// A strong salt to use (256 bit)
    /// </summary>
    /// <returns>A salt for usage in strong cryptography function</returns>
    /// <inheritdoc />
    public string Salt() { return Salt(64); }

    /// <summary>
    /// A strong salt to use
    /// </summary>
    /// <param name="length">The length of the string in bytes (hex value is returned)</param>
    /// <returns>A salt for usage in strong cryptography function</returns>
    /// <inheritdoc />
    public string Salt(int length)
    {
        return Salt(length, Convert.ToBase64String);
    }

    /// <summary>
    /// A strong salt to use
    /// </summary>
    /// <param name="length">The length of the string in bytes (hex value is returned)</param>
    /// <param name="converter">A byte to string converter to make a representation of the bytes</param>
    /// <returns>A salt for usage in strong cryptography function</returns>
    private static string Salt(int length, Func<byte[], string> converter)
    {
        var bytes = new byte[length];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(bytes);
        }
        return converter(bytes);
    }

    /// <summary>
    /// Encrypts a string, decrypt will be able to decrypt but only during the lifetime of the running of the application
    ///  Thus do not use for permanent encryption!
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <inheritdoc />
    public string Encrypt(string input)
    {
        var protector = _dataProtectionProvider.CreateProtector(Key);
        return protector.Protect(input);
    }

    /// <summary>
    /// Decrypts a string
    /// </summary>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    /// <inheritdoc />
    public string Decrypt(string cipherText)
    {
        try
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            var protector = _dataProtectionProvider.CreateProtector(Key);
            return protector.Unprotect(cipherText);
        }
        catch (Exception) { return string.Empty; }
    }

    #endregion

    #region Url Hashing

    /// <summary>
    /// More low level hasher, return a hex string of data provided a hashing algorithm and bytes
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <inheritdoc />
    public string UrlHash(string data)
    {
        var value = Encoding.UTF8.GetBytes(data);
        return ByteArrayToString(SHA256.Create().ComputeHash(value));
    }

    /// <summary>
    /// Returns if the provided hash a valid url (sha512) hash given the data and the salt
    /// </summary>
    /// <param name="hash">The hash the check</param>
    /// <param name="data">The data that was used for hashing</param>
    /// <param name="salt">The salt that was used during hashing</param>
    /// <returns>True when it is a valid hash, else false</returns>
    /// <inheritdoc />
    public bool IsValidUrlHash(string hash, string data, string salt)
    {
        return hash == UrlHash(data + salt);
    }

    /// <summary>
    /// Returns if the provided hash a valid url (sha512) hash given the data
    /// </summary>
    /// <param name="hash">The hash the check</param>
    /// <param name="data">The data that was used for hashing</param>
    /// <returns>True when it is valid hash, else false</returns>
    /// <inheritdoc />
    public bool IsValidUrlHash(string hash, string data) => hash == UrlHash(data);

    /// <summary>
    /// The string value to provide a salted hash for.
    /// </summary>
    /// <param name="value">The value to hash</param>
    /// <param name="salt">The salt to use</param>
    /// <returns>A strong (like SHA256 type) hash of value with a salt included</returns>
    /// <inheritdoc />
    public string UrlHash(string value, string salt) => UrlHash(value + salt);

    /// <summary>
    /// Create a protect querying string
    /// </summary>
    /// <param name="key">The key to use in the query string</param>
    /// <param name="data">The data to hash and to include in the query string</param>
    /// <returns>A query string with a hash protection measure</returns>
    /// <inheritdoc />
    public string ProtectedQueryString(string key, string data)
    {
        // ensure lowercase keys to avoid confusion
        key = key.ToLower();

        // Issues SEF-6: in (time) protected keys some keys are reserved
        if (key == "r" || key == "h") throw new ArgumentException("{key} is not an allowed key for data protection");

        // create all the ingredients
        var random = Salt(32, ByteArrayToString);

        // create the hash
        var hash = UrlHash(data + random, _options.UrlProtectionKey);

        // create the query string
        return $"r={random}&{key}={data}&h={hash}";
    }

    /// <summary>
    /// Test if the provided information represent a correct query string
    /// </summary>
    /// <param name="data">The data to hash and included in the query string</param>
    /// <param name="random">The random salt used during generation</param>
    /// <param name="hash">The hash which includes the random, date and data</param>
    /// <returns>True when correct else false</returns>
    /// <inheritdoc />
    public bool IsProtectedQueryString(string data, string random, string hash)
    {
        return hash == UrlHash(data + random, _options.UrlProtectionKey);
    }

    /// <summary>
    /// Create a protect querying string that will be valid for 24h providing a key and data
    /// </summary>
    /// <param name="key">The key to use in the query string</param>
    /// <param name="data">The data to hash and to include in the query string</param>
    /// <returns>A query string with all the protection measure</returns>
    /// <inheritdoc />
    public string TimeProtectedQueryString(string key, string data)
    {
        // ensure lowercase keys to avoid confusion
        key = key.ToLower();

        // Issues SEF-6: in (time) protected keys some keys are reserved
        if (key == "r" || key == "h" || key == "d") throw new ArgumentException("{key} is not an allowed key for data protection");

        // create all the ingredients
        var random = Salt(32, ByteArrayToString);
        var now = DateTime.UtcNow.ToString(DateTimeFormat);

        // create the hash
        var hash = UrlHash(data + random + now, _options.UrlProtectionKey);

        // create the query string
        return $"r={random}&d={now}&{key}={data}&h={hash}";
    }

    /// <summary>
    /// Test if the provided information represent a correct query string
    /// </summary>
    /// <param name="data">The data to hash and included in the query string</param>
    /// <param name="random">The random salt used during generation</param>
    /// <param name="date">The date the query string was generated</param>
    /// <param name="hash">The hash which includes the random, date and data</param>
    /// <returns>True when correct else false</returns>
    /// <inheritdoc />
    public bool IsTimeProtectedQueryString(string data, string random, string date, string hash)
        => IsTimeProtectedQueryString(data, random, date, hash, _options.TokenDurationInt * 3600);

    /// <summary>
    /// Test if the provided information represent a correct query string
    /// </summary>
    /// <param name="data">The data to hash and included in the query string</param>
    /// <param name="random">The random salt used during generation</param>
    /// <param name="date">The date the query string was generated</param>
    /// <param name="hash">The hash which includes the random, date and data</param>
    /// <param name="duration">The max duration of the token in seconds</param>
    /// <returns>True when correct else false</returns>
    /// <inheritdoc />
    public bool IsTimeProtectedQueryString(string data, string random, string date, string hash, int duration)
    {
        var correct = DateTime.TryParseExact(date, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime then);
        if (correct == false) { return false; }
        if ((DateTime.UtcNow - then).TotalSeconds > duration) { return false; }
        var expected = UrlHash(data + random + date, _options.UrlProtectionKey);
        return expected == hash;
    }

    /// <summary>
    /// Test if the provided information represent a correct query string
    /// </summary>
    /// <param name="model">The model captured in the query string</param>
    /// <returns>True when correct else false</returns>
    /// <inheritdoc />
    public bool IsTimeProtectedQueryString(ITimeProtectedModel model)
    {
        return IsTimeProtectedQueryString(model.Data, model.Random, model.Date, model.Hash);
    }

    /// <summary>
    /// Creates a hash that protected some data by a salt and a hash.
    /// </summary>
    /// <param name="data">The data to hash</param>
    /// <param name="time">The created time for the hash</param>
    /// <returns>A hash of the data </returns>
    /// <inheritdoc />
    public string TimeProtectedHash(string data, out string time)
    {
        time = DateTime.UtcNow.ToString(DateTimeFormat);
        return UrlHash(data + time, _options.UrlProtectionKey);
    }

    /// <summary>
    /// Verify the hash that was created protecting data
    /// </summary>
    /// <param name="hash"/>
    /// <param name="data">The data to hash</param>
    /// <param name="time">The time the hash was created</param>
    /// <returns>True when hash was correct else false</returns>
    /// <inheritdoc />
    public bool IsTimeProtectedHash(string hash, string data, string time)
    {
        return hash == UrlHash(data + time, _options.UrlProtectionKey);
    }

    /// <summary>
    /// Creates a hash that protected some data by a salt, random and a hash.
    /// </summary>
    /// <param name="data">The data to hash</param>
    /// <param name="time">The created time for the hash</param>
    /// <param name="random">The hashed date</param>
    /// <returns>A hash of the data </returns>
    /// <inheritdoc />
    public string TimeRandomProtectedHash(string data, out string time, out string random)
    {
        random = Salt();
        return TimeProtectedHash(random + data, out time);
    }

    /// <summary>
    /// Verify the hash that was created protecting data using salt, random and time
    /// </summary>
    /// <param name="hash">The created has</param>
    /// <param name="data">The data to hash</param>
    /// <param name="time">The time the hash was created</param>
    /// <param name="random">The hashed date</param>
    /// <returns>True when hash was correct else false</returns>
    /// <inheritdoc />
    public bool IsTimeRandomProtectedHash(string hash, string data, string time, string random)
    {
        return IsTimeProtectedHash(hash, random + data, time);
    }

    /// <summary>
    /// This method converts bytes into a hex string
    /// </summary>
    /// <param name="ba">Array with bytes to create a hash for</param>
    /// <returns></returns>
    private static string ByteArrayToString(byte[] ba)
    {
        var hex = new StringBuilder(ba.Length * 2);
        foreach (var b in ba) { hex.AppendFormat("{0:x2}", b); }
        return hex.ToString();
    }

    #endregion
}
