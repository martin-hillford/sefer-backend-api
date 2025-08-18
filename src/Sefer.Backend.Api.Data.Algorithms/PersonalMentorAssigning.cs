namespace Sefer.Backend.Api.Data.Algorithms;

public class PersonalMentorAssigning(PersonalMentorAssigningInput input) : IMentorAssigningAlgorithm
{
    /// <summary>
    /// This method returns the assigned mentor
    /// </summary>
    public User? GetMentor()
    {
        // Calculate for each mentor a score and rank them
        var ranked = GetRanked();
        
        // In general, there is a preference for the same gender. So first check if a mentor is available in the top x
        // with the same gender. If there is a strict gender preference, then not just take the top, but all
        var take = input.StrictGender ? ranked.Count : 100;
        var preferred = ranked.Where(mentor => mentor.Gender == input.Student.Gender).Take(take).FirstOrDefault();
        if(preferred != null) return preferred;

        // If there is not a mentor available in the same gender, it depends on the StrictGender settings what to do
        return input.StrictGender switch
        {
            // with strict gender and the back mentor has the same gender, return the backup mentor
            true when input.BackupMentor?.Gender == input.Student.Gender => input.BackupMentor,
            
            // with strict gender return no mentor  
            true => null,
            
            // if there is no strict gender policy, return the highest ranking mentor
            false when ranked.Count > 0 => ranked.First(),
            
            // if there is no mentor available, but a backup mentor is, return the backup mentor
            false when input.BackupMentor != null => input.BackupMentor,
            
            // out of options...
            _ => null
        };
    }

    // Rank the mentors based on their score
    private List<User> GetRanked()
    {
        var availabilityRanked = GetAvailabilityRanked();
        var ageRanked = GetAgeRanked();

        var scores = availabilityRanked
            .Select(score => new
            {
                mentor = score.Key,
                totalScore =
                    score.Value * input.RelativeAvailabilityFactor +
                    ageRanked[score.Key] * input.RelativeAgeFactor,
                availability = score.Value,
                relativeAge = ageRanked[score.Key],
                mentorId = score.Key.Id
            })
            .OrderByDescending(score => score.totalScore)
            .ToList();
            
        return scores.Select(score => score.mentor)
            .ToList();
    }
    
    // Calculates for each mentor the availability rank. 
    private Dictionary<User, int> GetAvailabilityRanked() =>
        input.StudentsPerMentor
            .Where(mentor => mentor.Key.MentorSettings.MaximumStudents > 0) // prevent a division by zero error
            .Where(mentor => mentor.Value <= mentor.Key.MentorSettings.MaximumStudents)
            .OrderByDescending(mentor => 1 - (mentor.Value / mentor.Key.MentorSettings.MaximumStudents))
            .Select((mentor, index) => new { mentor = mentor.Key, index })
            .ToDictionary(x => x.mentor, x => x.index);

    // Calculates for each mentor the age rank. 
    // By ordering descending first on the id of the mentor, mentors that are added last (and thus have properly not 
    // yet student or few students) are getting a slightly higher score 
    private Dictionary<User, int> GetAgeRanked() =>
        input.Mentors
            .OrderBy(mentor => Math.Abs(mentor.YearOfBirth - input.Student.YearOfBirth - input.OptimalAgeDifference))
            .ThenByDescending(mentor => mentor.Id)
            .Select((mentor, index) => new { mentor, index })
            .ToDictionary(x => x.mentor, x => x.index);
    
}