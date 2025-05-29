using System.Text.Json.Serialization;

namespace Sefer.Backend.Api.Data.Handlers;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}