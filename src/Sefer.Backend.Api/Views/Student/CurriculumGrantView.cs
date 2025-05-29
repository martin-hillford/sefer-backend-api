// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Data.Models.Courses.Curricula;
using Sefer.Backend.Api.Data.Models.Courses.Rewards;

namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// This view depicts the grant awarded on a full curriculum
/// </summary>
public class CurriculumGrantView : AbstractView<RewardGrant>
{
    /// <summary>
    /// Holds the code of the grant
    /// </summary>
    public string Code => Model.Code;

    /// <summary>
    /// The name of the curriculum this grant is about
    /// </summary>
    public readonly string CurriculumName;

    /// <summary>
    /// Create a new view
    /// </summary>
    /// <param name="grant"></param>
    /// <param name="curriculum"></param>
    public CurriculumGrantView(RewardGrant grant, Curriculum curriculum) : base(grant)
    {
        CurriculumName = curriculum.Name;
    }
}