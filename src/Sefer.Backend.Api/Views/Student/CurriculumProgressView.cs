// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Data.Models.Courses.Curricula;
using Sefer.Backend.Api.Views.Shared.Courses.Curricula;

namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// Shows the progress of the curriculum
/// </summary>
public class CurriculumProgressView : CurriculumView
{
    /// <summary>
    /// Holds the progress for the student (a percentage)
    /// </summary>
    public readonly float Progress;

    /// <summary>
    /// Create a view on the progress of a student on the curriculum
    /// </summary>
    /// <param name="model"></param>
    /// <param name="progress"></param>
    /// <returns></returns>
    public CurriculumProgressView(Curriculum model,float progress) : base(model)
    {
        Progress = progress;
    }
}