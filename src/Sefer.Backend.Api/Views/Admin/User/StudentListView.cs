// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Views.Admin.User;

/// <summary>
/// A view on student containing if the student is active or not
/// </summary>
/// <inheritdoc />
public class StudentListView : UserListView
{
    /// <summary>
    /// Contains is this student is an active student
    /// </summary>
    public readonly bool IsActive;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="active"></param>
    /// <inheritdoc />
    public StudentListView(Data.Models.Users.User model, bool active) : base(model)
    {
        IsActive = active;
    }
}