// ReSharper disable ConstantConditionalAccessQualifier
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Sefer.Backend.Api.Shared;

namespace Sefer.Backend.Api.Support;

public static class UserSettingsHelper
{
    public static ActionResult ToJson<T>(T view, List<UserSetting> userSettings, string subProperty) where T : class
    {
        // Serialize the object to a JSON string and parse into a mutable structure
        var options = DefaultJsonOptions.GetOptions();
        var jsonString = JsonSerializer.Serialize(view, options);
        var jsonNode = JsonNode.Parse(jsonString);
        if(jsonNode is null) throw new Exception("JsonNode is null");
        var jsonObject = jsonNode.AsObject();
        
        // Now add to the jsonObject all the properties from the settings
        var settingsObject = subProperty != null ? jsonObject[subProperty]?.AsObject() : jsonObject;
        if(settingsObject is null) throw new Exception("settingsObject is null");
        
        foreach (var userSetting in userSettings)
        {
            switch (userSetting.Value?.ToLower())
            {
                case "false":
                    settingsObject.Add(userSetting.KeyName, false);
                    break;
                case "true":
                    settingsObject.Add(userSetting.KeyName, true);
                    break;
                default:
                    settingsObject.Add(userSetting.KeyName, userSetting.Value);
                    break;
            }
        }
        
        // Create again JSON from the string
        var json = jsonObject.ToJsonString(options);
        return new ContentResult {  Content = json,  ContentType = "application/json" };
    }
    
    public static async Task<T> FromJson<T>(IHttpContextAccessor httpContextAccessor, List<UserSetting> userSettings)  where T : class
    {
        try
        {
            var jsonString = await GetBody(httpContextAccessor);
            if (jsonString == null) return null;

            var view = JsonSerializer.Deserialize<T>(jsonString, DefaultJsonOptions.GetOptions());
            if (view == null) return null;
            
            var document = JsonDocument.Parse(jsonString);
            userSettings.ForEach(c => SetValue(document, c));
            return view;
        }
        catch (Exception) { return null; }
    }
    
    private static void SetValue(JsonDocument document, UserSetting userSetting)
    {
        userSetting.Value = GetStringValue(document, userSetting);
    }
    
    private static string GetStringValue(JsonDocument document, UserSetting userSetting)
    {
        var useOldScheme = document.RootElement.TryGetProperty(userSetting.KeyName, out var oldSchemeValue);
        if(useOldScheme) return GetStringValue(oldSchemeValue, userSetting);

        var useNewScheme = document.RootElement.TryGetProperty("settings", out var settings);
        if(!useNewScheme) return userSetting.Value;
        
        var hasValue = settings.TryGetProperty(userSetting.KeyName, out var newSchemeValue);
        return hasValue ? GetStringValue(newSchemeValue, userSetting) : userSetting.Value;
    }
    
    private static string GetStringValue(JsonElement element, UserSetting setting)
    {
        var value = element.GetRawText().Replace("\"", string.Empty);
        if (value?.ToLower() == "True" || value?.ToLower() == "False") value = value.ToLower();
        return string.IsNullOrEmpty(value) ? setting.Value : value;
    }
    
    private static async Task<string> GetBody(IHttpContextAccessor httpContextAccessor)
    {
        var body =  httpContextAccessor.HttpContext?.Request.Body;
        if(body == null) return null;
        using var reader = new StreamReader(body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}