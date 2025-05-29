namespace Sefer.Backend.Api.Data.Models.Validation;

/// <summary>
/// MultiSelectQuestionAttribute is capable of resolving if a multiple choice question
/// has only one correct answer when it's not a multi select question
/// </summary>
public class MultiSelectQuestionAttribute : ValidationAttribute
{
    /// <summary>
    /// Validates if the number of correct answer is validate given multi selection
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Test if the object type implements the IMultipleChoiceQuestion interface
        if (value == null) return InterfaceFail(validationContext);
        var isMultiSelect = (bool)value;
            
        if (!typeof(IMultipleChoiceQuestion).IsAssignableFrom(validationContext.ObjectType))
        {
            return InterfaceFail(validationContext);
        }
        
        // Check if the choices are loaded. If that is not the case, this validation can not happen
        // Then the validation is set a being a success
        var choicesProperty = validationContext.ObjectType.GetProperty("Choices");
        var choicesValue = choicesProperty?.GetValue(validationContext.ObjectInstance, null);
        if(choicesValue is not List<MultipleChoiceQuestionChoice> choices || choices.Count == 0) return ValidationResult.Success;

        // Get the id of the object to validate the permalink for
        var countProperty = validationContext.ObjectType.GetProperty("CorrectAnswerCount");
        var correctAnswerCount = countProperty?.GetValue(validationContext.ObjectInstance, null);
        if (correctAnswerCount == null) return new ValidationResult("CorrectAnswerCount property missing");
            
        // Test is the number of correct answer is zero or one is the question is not a multi select question
        if (isMultiSelect || (int)correctAnswerCount == 1) return ValidationResult.Success;
        return new ValidationResult(FormatErrorMessage("IsMultiSelect"), new List<string> { "IsMultiSelect" });
    }

    private static ValidationResult InterfaceFail(ValidationContext validationContext)
    {
        return new ValidationResult(validationContext.ObjectType.FullName + " should implemented the IMultipleChoiceQuestion interface");
    }
}