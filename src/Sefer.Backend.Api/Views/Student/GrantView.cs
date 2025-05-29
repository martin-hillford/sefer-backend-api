using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Data.Models.Courses.Rewards;

namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// This view depicts the grant awarded at the moment of submission
/// </summary>
public class GrantView : AbstractView<RewardGrant>
{
    /// <summary>
    /// Create a new view
    /// </summary>
    /// <param name="grant"></param>
    public GrantView(RewardGrant grant) : base(grant) { }
}