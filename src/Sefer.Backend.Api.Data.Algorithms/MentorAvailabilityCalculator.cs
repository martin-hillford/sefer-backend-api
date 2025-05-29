namespace Sefer.Backend.Api.Data.Algorithms;

public static class MentorAvailabilityCalculator
{
    public static MentorAvailabilityResult GetScores(MentorAssigningInput input)
    {
        // First get the individual scores
        var preferredAvailability = GetPreferredAvailabilityScores(input);
        var maximumAvailability = GetMaximumAvailabilityScores(input);
        var ageDistance = GetAgeDistanceScores(input);

        // Now create the complete scores for each mentor
        var scores = new List<MentorAvailabilityScore>();
        foreach (var mentor in input.Mentors.Values)
        {
            var preferred = preferredAvailability.GetValueOrDefault(mentor, null);
            var maximum = maximumAvailability.GetValueOrDefault(mentor, null);
            var distance = ageDistance.GetValueOrDefault(mentor, null) ?? double.MaxValue;

            var score = new MentorAvailabilityScore(mentor)
            {
                PreferredAvailability = preferred,
                MaximumAvailability = maximum,
                AgeDistance = distance,
                SameGender = mentor.Gender == input.Student.Gender,
                MaximumCombinationScore = Combine(input, distance, maximum),
                PreferredCombinationScore = Combine(input, distance, preferred)
            };
            scores.Add(score);
        }

        return new MentorAvailabilityResult(scores);
    }

    private static Dictionary<User, double?> GetPreferredAvailabilityScores(MentorAssigningInput input)
    {
        var scores = new Dictionary<User, double?>();
        foreach (var mentor in input.Mentors.Values)
        {
            double preferred = mentor.MentorSettings.PreferredStudents;
            double actual = input.ActiveStudents.GetActiveStudents(mentor.Id);
            if (actual >= preferred) continue;
            scores.Add(mentor, actual / preferred);
        }
        return ScaleScores(scores, 1);
    }

    private static Dictionary<User, double?> GetMaximumAvailabilityScores(MentorAssigningInput input)
    {
        var scores = new Dictionary<User, double?>();
        foreach (var mentor in input.Mentors.Values)
        {
            double maximum = mentor.MentorSettings.MaximumStudents;
            double preferred = mentor.MentorSettings.PreferredStudents;
            double actual = input.ActiveStudents.GetActiveStudents(mentor.Id);
            if (actual >= maximum || actual < preferred) continue;
            scores.Add(mentor, actual / preferred);
        }
        return ScaleScores(scores, 1);
    }

    private static Dictionary<User, double?> GetAgeDistanceScores(MentorAssigningInput input)
    {
        // First step is calculate the absolute age distance for each mentor / student combination
        var scores = new Dictionary<User, double?>();
        var max = 0;
        foreach (var mentor in input.Mentors.Values)
        {
            var distance = GetAgeDistanceScore(mentor, input.Student, input.OptimalAgeDifference);
            scores.Add(mentor, distance);
            max = Math.Max(max, distance);
        }

        // Foreach score simply divide it to a percentage
        if (max == 0) return ScaleScores(scores, 1);
        foreach (var (key, score) in scores) { scores[key] = score / max; }

        // Scale the scores so they can be compared
        return ScaleScores(scores, 1);
    }

    private static int GetAgeDistanceScore(User mentor, User student, int optimalDistance)
    {
        var diff = Math.Abs(student.YearOfBirth - mentor.YearOfBirth);
        return Math.Abs(diff - optimalDistance);
    }

    private static Dictionary<User, double?> ScaleScores(Dictionary<User, double?> scores, double xRange)
    {
        if (scores.Count == 0 || xRange == 0) return scores;

        // Please note: see https://www.theanalysisfactor.com/rescaling-variables-to-be-same/ for the used technique
        var xMin = scores.Values.Min();
        foreach (var (key, score) in scores)
        {
            scores[key] = (score - xMin) / xRange;
        }
        return scores;
    }

    private static double? Combine(MentorAssigningInput input, double ageDistance, double? availability)
    {
        if (availability == null) return null;
        return
            (input.RelativeAvailabilityFactor * availability.Value) +
            (input.RelativeAgeFactor * ageDistance);
    }
}