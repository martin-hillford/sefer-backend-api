// ReSharper disable UseNegatedPatternMatching

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Sefer.Backend.Api.Shared.Validation;

/// <summary>
/// 
/// </summary>
/// <param name="allowedKeysConfig"></param>
/// <param name="maxObjectSize"></param>
public class JsonDictionaryAttribute(string allowedKeysConfig, int maxObjectSize = int.MaxValue) : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if(value == null) return ValidationResult.Success;
        
        // Check the type 
        var dictionary = value as Dictionary<string, JsonElement>;
        if(dictionary == null) return new ValidationResult("The value is not a dictionary");
        
        // Check if only allowed keys are present in the list
        var configuration = validationContext.GetService<IConfiguration>();
        var allowed = GetAllowedKeys(configuration);
        var keysValid = dictionary.Keys.All(k => allowed.Contains(k));
        if(!keysValid) return new ValidationResult("Not all keys are not allowed");

        // Also prevent that the user will use this for personal storage
        var json = JsonSerializer.Serialize(dictionary, DefaultJsonOptions.GetOptions());
        return json.Length <= maxObjectSize
            ? ValidationResult.Success
            : new ValidationResult($"To much data stored in object, maxSize {maxObjectSize}");
    }

    private HashSet<string> GetAllowedKeys(IConfiguration configuration)
    {
        try { return configuration.GetSection(allowedKeysConfig).Get<string[]>().ToHashSet(); }
        catch { return []; }
    }
}