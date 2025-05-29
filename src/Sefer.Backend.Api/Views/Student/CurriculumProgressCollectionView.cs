namespace Sefer.Backend.Api.Views.Student;

/// <summary>
///  A view for the curriculum progress collection
/// </summary>
public class CurriculumProgressCollectionView
{
    /// <summary>
    /// Information on the curricula
    /// </summary>
    public List<CurriculumProgressView> Curricula { get; init; }

    /// <summary>
    /// Information on the grants
    /// </summary>
    public List<CurriculumGrantView> Grants { get; init; }
}