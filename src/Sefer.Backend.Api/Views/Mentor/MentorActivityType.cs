// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// Defines the different types of activity for the stream
/// </summary>
public enum MentorActivityType : byte
{
    /// <summary>
    /// Indicates the logon of the user
    /// </summary>
    Login,

    /// <summary>
    /// Indicate the user has created his profile
    /// </summary>
    ProfileCreation
}