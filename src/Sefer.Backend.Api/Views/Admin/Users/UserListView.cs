// This is a view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;
using User = Sefer.Backend.Api.Data.Models.Users.User;

namespace Sefer.Backend.Api.Views.Admin.Users;

/// <summary>
/// A base view for a user for usage in lists for the admin
/// </summary>
/// <inheritdoc />
public class UserListView(User model) : UserView(model)
{
    /// <summary>
    /// Store for the underlying model
    /// </summary>
    protected readonly User Model = model;

    /// <summary>
    /// Gets if the user is blocked
    /// </summary>
    public bool Blocked => Model.Blocked;

    /// <summary>
    /// Gets if the user is not blocked
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
}
