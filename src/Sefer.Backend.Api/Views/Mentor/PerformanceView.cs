// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// Information on the performance of the mentor
/// </summary>
public class PerformanceView : PrimitiveUserView
{
    /// <summary>
    /// The underlying data model
    /// </summary>
    private readonly MentorPerformance _performanceReport;

    /// <summary>
    /// Create a performance view
    /// </summary>
    /// <param name="mentor"></param>
    /// <param name="performance"></param>
    /// <returns></returns>
    public PerformanceView(User mentor, MentorPerformance performance) : base(mentor)
    {
        _performanceReport = performance;
    }

    /// <summary>
    /// The average message the mentor sends to each student
    /// </summary>
    public int? AverageMessagePerStudent => _performanceReport.AverageMessagePerStudent;

    /// <summary>
    /// The average time (in minutes) the mentor takes to review a submitted lessons
    /// </summary>
    public int? ReviewTimeInMinutes => _performanceReport.ReviewTimeSpan;

    /// <summary>
    /// The days the mentor was active in the last year
    /// </summary>
    public int? DaysActive => _performanceReport.DaysActive;

    /// <summary>
    /// TThe average time (in minutes) the mentor takes to review a submitted lessons
    /// </summary>
    /// <value></value>
    public double? ReviewTimeInDays
    {
        get
        {
            if (_performanceReport.ReviewTimeSpan.HasValue == false) return null;
            return _performanceReport.ReviewTimeSpan.Value / (24d * 60d);
        }
    }

    /// <summary>
    /// The average rating of the mentor
    /// </summary>
    /// <value></value>
    public double? AverageRating
    {
        get
        {
            if (_performanceReport.AverageRating.HasValue) return Math.Round(_performanceReport.AverageRating.Value, 1);
            return null;
        }
    }
}