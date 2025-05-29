// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Admin.User;

/// <summary>
/// A base view for a user for usage in lists for the admin
/// </summary>
/// <inheritdoc />
public class UserListView : UserView
{
    /// <summary>
    /// The underlying model
    /// </summary>
    protected readonly Data.Models.Users.User Model;

    /// <summary>
    /// Gets if the user is blocked (by the admin)
    /// </summary>
    public bool Blocked => Model.Blocked;

    /// <summary>
    /// Gets if the user is not blocked (by the admin)
    /// </summary>
    public bool HasAccess => Model.Blocked == false;

    /// <summary>
    /// Gets if the user has activated his account
    /// </summary>
    public bool Approved => Model.Approved;

    /// <summary>
    /// Returns if the user has two-factor auth enabled
    /// </summary>
    public bool TwoFactorAuthEnabled => Model.TwoFactorAuthEnabled;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public UserListView(Data.Models.Users.User model) : base(model)
    {
        Model = model;
    }
}
