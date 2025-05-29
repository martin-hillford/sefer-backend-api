namespace Sefer.Backend.Api.Data;

public static class DefaultJsonOptions
{
    private static JsonSerializerOptions _options;

    public static JsonSerializerOptions GetOptions()
    {
        if (_options != null) return _options;
        _options = new JsonSerializerOptions();
        SetOptions(_options);
        return _options;
    }

    public static void SetOptions(JsonOptions options)
    {
        SetOptions(options.JsonSerializerOptions);
    }

    private static void SetOptions(JsonSerializerOptions options)
    {
        options.AllowTrailingCommas = true;
        options.IgnoreReadOnlyFields = false;
        options.IgnoreReadOnlyProperties = false;
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        options.PropertyNameCaseInsensitive = true;
        options.IncludeFields = true;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.ReadCommentHandling = JsonCommentHandling.Skip;
        options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.WriteIndented = false;
    }
}