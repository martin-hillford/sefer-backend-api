// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Algorithms;

public class MentorAvailabilityScore(User mentor)
{
    /// <summary>
    /// The scaled age distance score of the mentor.
    /// The ideal distance is 10 years with results is a score of 0
    /// </summary>
    public double AgeDistance { get; init; }

    /// <summary>
    /// The scaled availability score relative to the mentor preference.
    /// The lower the score, be more available a mentor is
    /// If the number of active students of the mentor is equal to or above the preferred settings this score is null
    /// </summary>
    public double? PreferredAvailability { get; init; }

    /// <summary>
    /// The scaled availability score elative to the mentor maximum.
    /// The lower the score, be more available a mentor is
    /// If the number of active students of the mentor is below the preferred settings this score is null
    /// Additional, if that number is above the maximum, this score is also null
    /// </summary>
    public double? MaximumAvailability { get; init; }

    /// <summary>
    /// The score for the mentor by combining age and maximum availability
    /// </summary>
    public double? MaximumCombinationScore { get; init; }

    /// <summary>
    /// The score for the mentor by combining age and preferred availability
    /// </summary>
    public double? PreferredCombinationScore { get; init; }

    /// <summary>
    /// True when them mentor and the student have the same gender
    /// </summary>
    public bool SameGender { get; init; }

    /// <summary>
    /// The mentor for which this score is calculated
    /// </summary>
    public readonly User Mentor = mentor;
}