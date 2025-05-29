using System.Collections;

namespace Sefer.Backend.Api.Data.Models.Validation;

/// <summary>
/// Validates the number of elements in a list
/// </summary>
/// <inheritdoc />
public class CountAttribute : ValidationAttribute
{
    /// <summary>
    /// The minimum number of items in the list
    /// </summary>
    private readonly int _min;

    /// <summary>
    /// The maximum number of items in the list
    /// </summary>
    private readonly int _max;

    /// <summary>
    /// Creates a new count attribute
    /// </summary>
    /// <param name="count">The number of elements that are required at least to be in the collection</param>
    /// <inheritdoc />
    public CountAttribute(int count)
    {
        _min = count;
        _max = int.MaxValue;
    }

    /// <summary>
    /// Creates a new count attribute
    /// </summary>
    /// <param name="min">The minimum number of items in the list</param>
    /// <param name="max">The maximum number of items in the list</param>
    /// <inheritdoc />
    // ReSharper disable once UnusedMember.Global
    public CountAttribute(int min, int max)
    {
        _min = min;
        _max = max;
    }

    /// <summary>
    /// Validates if the list has the required count
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    /// <inheritdoc />
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        return value switch
        {
            null => new ValidationResult("Value cannot be null"),
            IList list when list.Count >= _min && list.Count <= _max => ValidationResult.Success,
            _ => base.IsValid(null, validationContext)
        };
    }
}