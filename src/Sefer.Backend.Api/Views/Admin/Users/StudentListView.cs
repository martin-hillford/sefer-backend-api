// This is a view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Views.Admin.Users;

/// <summary>
/// A view on a student containing if the student is active or not
/// </summary>
/// <inheritdoc />
public class StudentListView(User model, bool active) : UserListView(model)
{
    /// <summary>
    /// Contains is this student is an active student
    /// </summary>
    public readonly bool IsActive = active;
}