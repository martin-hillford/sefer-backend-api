namespace Sefer.Backend.Api.Data.Models.Validation;

/// <summary>
/// The CustomFunctionAttribute will check on custom function is they are existing function (static)
/// </summary>
public class CustomFunctionAttribute : ValidationAttribute
{
    /// <summary>
    /// Validates if the Permalink (value) is unique
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Get the custom function
        var customFunction = string.Empty;
        if(value != null) { customFunction = value.ToString(); }

        // Check for the custom function
        var result = ValidationResult.Success;
        if (customFunction != string.Empty)
        {
            throw new NotImplementedException("CustomFunction are not yet implemented in Sefer.");
        }
        return result;
    }
}