namespace Sefer.Backend.Api.Data.Models.Constants;

/// <summary>
/// Defines the type of pages
/// </summary>
public enum ContentPageType : short
{
    /// <summary>
    /// The about us root page is a page that shows up at the root of about us page.
    /// </summary>
    /// <remarks>Only one allowed</remarks>
    HomePage = 0,

    /// <summary>
    /// The privacy statement page is a page that will contain a statement about the privacy of users
    /// </summary>
    /// <remarks>Only one allowed</remarks>
    PrivacyStatementPage = 1,

    /// <summary>
    /// There also can be individual pages that can linked on web or other places and don't show up in the menu
    /// </summary>
    IndividualPage = 2,

    /// <summary>
    /// Pages that show up in the menu
    /// </summary>
    MenuPage = 3,

    /// <summary>
    /// The privacy statement page is a page that will contain a statement about the terms of usage
    /// </summary>
    /// <remarks>Only one allowed</remarks>
    UsageTermsPage = 4,
}

