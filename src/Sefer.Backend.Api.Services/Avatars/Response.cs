// ReSharper disable PropertyCanBeMadeInitOnly.Global

using System.Text.Json.Serialization;

namespace Sefer.Backend.Api.Services.Avatars;

public class Response
{
    public string Base64 { get; set; }

    public string Content { get; set; }

    public string ContentType { get; set; }

    public DateTime? Expires { get; set; }

    [JsonIgnore]
    public bool IsBase64 => !string.IsNullOrEmpty(Base64);

    [JsonIgnore]
    public bool HasImage => (!string.IsNullOrEmpty(Base64) || !string.IsNullOrEmpty(Content)) && !string.IsNullOrEmpty(ContentType);
    
    public static Response Empty() => new() { Expires = DateTime.UtcNow.AddHours(12) };

    public static Response FromBase64(string base64, string contentType, DateTime? expires = null)
        => new () { Base64 = base64, ContentType = contentType, Expires = expires };

    public static Response FromString(string content, string contentType, DateTime? expires = null)
        => new() { Content = content, ContentType = contentType, Expires = expires };
}