// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Services.Security.Abstractions;

/// <summary>
/// The definition of a cryptography service which should provide strong cryptography function
/// </summary>
public interface ICryptographyService
{
    /// <summary>
    /// The string value to provide a salted hash for.
    /// </summary>
    /// <param name="value">The value to hash</param>
    /// <param name="salt">The salt to use, should be in a base64 format!</param>
    /// <returns>A strong (like SHA256 type) hash of value with a salt included</returns>
    string Hash(string value, string salt);

    /// <summary>
    /// Returns a hash from the given value (which will also be used as salt)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    string Hash(string value);

    /// <summary>
    /// A strong salt to use (256 bit)
    /// </summary>
    /// <returns>A salt for usage in strong cryptography function</returns>
    string Salt();

    /// <summary>
    /// A strong salt to use
    /// </summary>
    /// <param name="length">The length of the string in bytes (hex value is returned)</param>
    /// <returns>A salt for usage in strong cryptography function</returns>
    string Salt(int length);

    /// <summary>
    /// Encrypts a string, decrypt will be able to decrypt but only during the lifetime of the running of the application
    ///  Thus do not use for permanent encryption!
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    string Encrypt(string input);

    /// <summary>
    /// Decrypts a string
    /// </summary>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    string Decrypt(string cipherText);

    /// <summary>
    /// Create a protect querying string
    /// </summary>
    /// <param name="key">The key to use in the query string</param>
    /// <param name="data">The data to hash and to include in the query string</param>
    /// <returns>A query string with a hash protection measure</returns>
    string ProtectedQueryString(string key, string data);

    /// <summary>
    /// Create a time protect querying string that will be valid for 24h providing a key and data
    /// </summary>
    /// <param name="key">The key to use in the query string</param>
    /// <param name="data">The data to hash and to include in the query string</param>
    /// <returns>A query string with all the protection measure</returns>
    string TimeProtectedQueryString(string key, string data);

    /// <summary>
    /// Test if the provided information represent a correct query string
    /// </summary>
    /// <param name="data">The data to hash and included in the query string</param>
    /// <param name="random">The random salt used during generation</param>
    /// <param name="hash">The hash which includes the random, date and data</param>
    /// <returns>True when correct else false</returns>
    bool IsProtectedQueryString(string data, string random, string hash);

    /// <summary>
    /// Test if the provided information represent a correct query string
    /// </summary>
    /// <param name="data">The data to hash and included in the query string</param>
    /// <param name="random">The random salt used during generation</param>
    /// <param name="date">The date the query string was generated</param>
    /// <param name="hash">The hash which includes the random, date and data</param>
    /// <returns>True when correct else false</returns>
    bool IsTimeProtectedQueryString(string data, string random, string date, string hash);

    /// <summary>
    /// Test if the provided information represent a correct query string
    /// </summary>
    /// <param name="data">The data to hash and included in the query string</param>
    /// <param name="random">The random salt used during generation</param>
    /// <param name="date">The date the query string was generated</param>
    /// <param name="hash">The hash which includes the random, date and data</param>
    /// <param name="duration">The max duration of the token in seconds</param>
    /// <returns>True when correct else false</returns>
    public bool IsTimeProtectedQueryString(string data, string random, string date, string hash, int duration);

    /// <summary>
    /// Test if the provided information represent a correct query string
    /// </summary>
    /// <param name="model">The model captured in the query string</param>
    /// <returns>True when correct else false</returns>
    bool IsTimeProtectedQueryString(ITimeProtectedModel model);

    /// <summary>
    /// Creates a hash that protected some data by a salt and a hash.
    /// </summary>
    /// <param name="data">The data to hash</param>
    /// <param name="time">The created time for the hash</param>
    /// <returns>A hash of the data </returns>
    string TimeProtectedHash(string data, out string time);

    /// <summary>
    /// Verify the hash that was created protecting data
    /// </summary>
    /// <param name="hash"/>
    /// <param name="data">The data to hash</param>
    /// <param name="time">The time the hash was created</param>
    /// <returns>True when hash was correct else false</returns>
    bool IsTimeProtectedHash(string hash, string data, string time);

    /// <summary>
    /// Creates a hash that protected some data by a salt, random and a hash.
    /// </summary>
    /// <param name="data">The data to hash</param>
    /// <param name="time">The created time for the hash</param>
    /// <param name="random">The hashed date</param>
    /// <returns>A hash of the data </returns>
    string TimeRandomProtectedHash(string data, out string time, out string random);

    /// <summary>
    /// Verify the hash that was created protecting data using salt, random and time
    /// </summary>
    /// <param name="hash">The created has</param>
    /// <param name="data">The data to hash</param>
    /// <param name="time">The time the hash was created</param>
    /// <param name="random">The hashed date</param>
    /// <returns>True when hash was correct else false</returns>
    bool IsTimeRandomProtectedHash(string hash, string data, string time, string random);

    /// <summary>
    /// Returns this the given hash is response hash for the given data
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    bool IsValidHash(string hash, string data);

    /// <summary>
    /// Returns this the given hash is response hash for the given data
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="data"></param>
    /// <param name="salt"></param>
    /// <returns>True if it is a valid hash, else false</returns>
    bool IsValidHash(string hash, string data, string salt);

    /// <summary>
    /// More low level hasher, return a hex string of data provided a hashing algorithm and bytes
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string UrlHash(string data);

    /// <summary>
    /// Returns if the provided hash a valid url (sha512) hash given the data and the salt
    /// </summary>
    /// <param name="hash">The hash the check</param>
    /// <param name="data">The data that was used for hashing</param>
    /// <param name="salt">The salt that was used during hashing</param>
    /// <returns>True when it is a valid hash, else false</returns>
    bool IsValidUrlHash(string hash, string data, string salt);

    /// <summary>
    /// Returns if the provided hash a valid url (sha512) hash given the data
    /// </summary>
    /// <param name="hash">The hash the check</param>
    /// <param name="data">The data that was used for hashing</param>
    /// <returns>True when it is a valid hash, else false</returns>
    bool IsValidUrlHash(string hash, string data);

    /// <summary>
    /// The string value to provide a salted hash for.
    /// </summary>
    /// <param name="value">The value to hash</param>
    /// <param name="salt">The salt to use</param>
    /// <returns>A strong (like SHA256 type) hash of value with a salt included</returns>
    string UrlHash(string value, string salt);
}
