// This is an entity framework model, so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Logging;

public class MentorPerformance
{
    /// <summary>
    /// The id of mentor this report is about
    /// </summary>
    public int MentorId { get; set; }

    /// <summary>
    /// The average message the mentor sends to each student
    /// </summary>
    public int? AverageMessagePerStudent { get; set; }

    /// <summary>
    /// The average time (in minutes) the mentor takes to review submitted lessons
    /// </summary>
    public int? ReviewTimeSpan { get; set; }

    /// <summary>
    /// The days the mentor was active in the last year
    /// </summary>
    public int? DaysActive { get; set; }

    /// <summary>
    /// The average rating of the mentor by the students
    /// </summary>
    public double? AverageRating { get; set; }
    
    /// <summary>
    /// The number of ratings given to the mentor
    /// </summary>
    public int? RatingCount { get; set; }
}