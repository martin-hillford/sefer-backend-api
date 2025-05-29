namespace Sefer.Backend.Api.Data.Models.Annotations;

/// <summary>
/// The format of the Permalink combines both a regular expression and a length of max 255 and min 1
/// </summary>
/// <inheritdoc cref="RegularExpressionAttribute"/>
public class PermalinkFormatAttribute : RegularExpressionAttribute
{
    /// <summary>
    /// The format used for a permalink
    /// </summary>
    public const string Format = @"^[a-z0-9\-]+$";

    /// <summary>
    /// Create a new FormatPermalinkAttribute providing the regular expression to the base
    /// </summary>
    /// <inheritdoc />
    public PermalinkFormatAttribute() : base(Format) { }

    /// <summary>
    /// Validates if the permalink is in the correct format  (single line, letters, numbers and hyphens only)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    /// <inheritdoc />
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Test if the object type implements the permalink interface
        if (typeof(IPermaLinkable).IsAssignableFrom(validationContext.ObjectType) == false)
        {
            throw new Exception(validationContext.ObjectType.FullName + " should implemented the PermaLinkable interface");
        }

        // Use the base to determine if the regular expression is matching
        if (value == null) return ValidationResult.Success;
        var result = base.IsValid(value);

        // If it's not a success return
        if (result == false) return new ValidationResult("The permalink is not the in the correct format.");

        var permalink = value.ToString();
        return permalink is not {Length: > 255} 
            ? ValidationResult.Success
            : new ValidationResult("A permalink cannot not be more then 255 characters long");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Global
    public ValidationResult IsValidObject(object value, ValidationContext validationContext) => IsValid(value,validationContext);
}