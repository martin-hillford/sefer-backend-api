namespace Sefer.Backend.Api.Data.Models.Constants;

/// <summary>
/// Defines the different types of results for a sign on of a user
/// </summary>
public enum SignOnResult : short { Success, Blocked, Unapproved, IncorrectSignIn, NoUser }