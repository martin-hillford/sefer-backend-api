namespace Sefer.Backend.Api.Data.Algorithms;

public class MentorAvailabilityResult(IEnumerable<MentorAvailabilityScore> scores)
{
    private readonly IReadOnlyCollection<MentorAvailabilityScore> _scores = scores.ToList().AsReadOnly();

    public User? GetMentorGivenMaximumScore(bool sameGender)
    {
        return GetMentorScore(Selector, sameGender);
        static double? Selector(MentorAvailabilityScore score) => score.MaximumCombinationScore;
    }

    public User? GetMentorGivenPreferredScore(bool sameGender)
    {
        return GetMentorScore(Selector, sameGender);
        static double? Selector(MentorAvailabilityScore score) => score.PreferredCombinationScore;
    }

    private User? GetMentorScore(Func<MentorAvailabilityScore, double?> selector, bool sameGender)
    {
        User? mentor = null;
        var lowest = 1d;

        foreach (var score in _scores)
        {
            if (sameGender != score.SameGender) continue;
            var current = selector(score);
            if (current == null || current >= lowest) continue;
            lowest = current.Value;
            mentor = score.Mentor;
        }

        return mentor;
    }
}