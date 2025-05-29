// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Admin.User;

/// <summary>
/// A view on mentors with regions
/// </summary>
/// <inheritdoc />
public class MentorWithRegionsView : UserView
{
    /// <summary>
    /// The list of regions that the mentor is in
    /// </summary>
    public readonly ReadOnlyCollection<IRegion> Regions;

    /// <summary>
    /// The list of regions that the mentor can be added to
    /// </summary>
    public readonly ReadOnlyCollection<IRegion> AvailableRegions;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="regions">The list of regions that is mentor is mentoring in </param>
    /// <param name="availableRegions">The list of regions that are available for mentoring</param>
    /// <inheritdoc />
    public MentorWithRegionsView(Data.Models.Users.User model, IEnumerable<IRegion> regions, IEnumerable<IRegion> availableRegions) : base(model)
    {
        Regions = regions.ToList().AsReadOnly();
        AvailableRegions = availableRegions.ToList().AsReadOnly();
    }
}
