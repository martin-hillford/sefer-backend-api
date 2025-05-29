namespace Sefer.Backend.Api.Data.Algorithms;

/// <summary>
/// This class implements the Mentor Assigning algorithm.
/// It is capable of selecting the mentor for a student based on:
/// last taken course, gender, age and availability
/// </summary>
/// <inheritdoc />
public class MentorAssigning : IMentorAssigningAlgorithm
{
    /// <summary>
    /// The student to which a mentor needs to be assigned
    /// </summary>
    private readonly MentorAssigningInput _input;

    /// <summary>
    /// Create a new instance of the mentor assigning algorithm
    /// </summary>
    public MentorAssigning(MentorAssigningInput input)
    {
        // Check and assign the student
        if (input.Student == null) throw new ArgumentNullException(nameof(input), "Student not provided");

        // Check and assign the mentors
        if (input.Mentors == null) throw new ArgumentNullException(nameof(input), "No mentors provided");
        if (input.Mentors.Count == 0) throw new ArgumentException("No mentors provided", nameof(input));

        // Check and assign the activateStudents
        if (input.ActiveStudents == null) throw new ArgumentNullException(nameof(input), "ActiveStudents not provided");

        // Assign the factor properties
        if (input.RelativeAgeFactor is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(input), "The relative age should between 0 and 1 (inclusive).");

        // check and assign the sameMentorDays
        if (input.SameMentorDays < 0) throw new ArgumentOutOfRangeException(nameof(input), "sameMentorDays should 0 or lager");

        // Input checked
        _input = input;
    }

    /// <summary>
    /// This method process the assigned data and returns the assigned mentor
    /// </summary>
    public User GetMentor()
    {
        // Rule 1: If the student has completed a course in equal to or less than sameMentorDays days in the past and
        //         the mentor availability is not change for the course then the student will have the same mentor
        var ruleOneMentor = GetMentorIfWithinReassignLimit();
        if (ruleOneMentor != null) return ruleOneMentor;

        // Rule 2: If the mentor of previous course (even if the student did not complete the course) is available,
        //         the student will keep that mentor
        var ruleTwoMentor = GetPreviousMentorIfAvailable();
        if (ruleTwoMentor != null) return ruleTwoMentor;

        // Next decisions are based upon the availability scores of the mentors
        var scores = MentorAvailabilityCalculator.GetScores(_input);

        // Rule 3: Find the mentor that has the same gender and the best preferred availability scores
        var ruleThreeMentor = scores.GetMentorGivenPreferredScore(true);
        if (ruleThreeMentor != null) return ruleThreeMentor;

        // Rule 4: Find the mentor that is has the same gender and the best maximum availability scores
        var ruleFourMentor = scores.GetMentorGivenMaximumScore(true);
        if (ruleFourMentor != null) return ruleFourMentor;

        // Rule 5: Find the mentor that is has not the same gender and the best preferred availability scores
        var ruleFiveMentor = scores.GetMentorGivenPreferredScore(false);
        if (ruleFiveMentor != null) return ruleFiveMentor;

        // Rule 6: Find the mentor that is has not the same gender and the best maximum availability scores
        var ruleSixMentor = scores.GetMentorGivenMaximumScore(false);
        if (ruleSixMentor != null) return ruleSixMentor;

        // Rule 7: If there are no available mentors, assign the student to the mentor with the highest overflow
        // mentor score index
        // Note: this is last resort method
        return GetOverFlowMentor() ?? _input.BackupMentor;
    }

    /// <summary>
    /// This method is determining if the user is enrolling within the time limit of assigning the same mentor
    /// </summary>
    private User? GetMentorIfWithinReassignLimit()
    {
        var enrollmentFinished = _input.LastStudentEnrollment is { IsCourseCompleted: true, MentorId: { } };
        var closureDate = _input.LastStudentEnrollment?.ClosureDate ?? DateTime.MinValue;
        var lastActiveDate = DateTime.UtcNow.AddDays(-1 * _input.SameMentorDays);
        var mentorId = _input.LastStudentEnrollment?.MentorId;

        // Check if the mentor is still active, the enrollment is finished and if the student is in time to get the same mentor
        if (enrollmentFinished == false) return null;
        if (closureDate >= lastActiveDate == false) return null;
        if (mentorId == null) return null;
        return _input.Mentors.GetValueOrDefault(mentorId.Value);
    }

    /// <summary>
    /// This method will determine if the mentor of the last enrollment is still available
    /// </summary>
    private User? GetPreviousMentorIfAvailable()
    {
        var mentorId = _input.LastStudentEnrollment?.MentorId;
        if (mentorId == null) return null;

        var mentor = _input.Mentors.GetValueOrDefault(mentorId.Value);
        if (mentor == null) return null;

        // Please note, -1 is used because the current student will be counted as active
        var students = _input.ActiveStudents.GetActiveStudents(mentor.Id) - 1;
        if (students < mentor.MentorSettings.MaximumStudents) return mentor;
        return null;
    }

    /// <summary>
    /// This method tries to find the overflow mentor
    /// </summary>
    private User? GetOverFlowMentor()
    {
        (User? overflow, var score) = (null, double.MaxValue);

        foreach (var mentor in _input.Mentors.Values)
        {
            if (!mentor.MentorSettings.AllowOverflow) continue;
            double students = _input.ActiveStudents.GetActiveStudents(mentor.Id);
            double maximum = mentor.MentorSettings.MaximumStudents;
            var current = students / maximum;

            if (current >= score) continue;
            score = current;
            overflow = mentor;
        }

        return overflow;
    }
}