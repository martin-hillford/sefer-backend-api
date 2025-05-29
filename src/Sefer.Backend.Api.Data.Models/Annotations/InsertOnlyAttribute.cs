namespace Sefer.Backend.Api.Data.Models.Annotations;

/// <summary>
/// The insert only attribute indicates a member (property) is only allowed for insert but not for update
/// </summary>
/// <inheritdoc cref="Attribute"/>
[AttributeUsage(AttributeTargets.Property)]
public class InsertOnlyAttribute : Attribute {  }