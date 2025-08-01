namespace Sefer.Backend.Api.Data.Algorithms;

public readonly struct PersonalMentorAssigningInput
{
    /// <summary>
    /// The student for which to determine a personal mentor
    /// </summary>
    public required User Student { get; init; }
    
    /// <summary>
    /// The list with mentors that are possibly available
    /// </summary>
    public required IReadOnlyList<User> Mentors { get; init; }
    
    /// <summary>
    /// Holds for each mentor the number of students he/she has.
    /// This should be based - in contrast with per-enrollment assignment - on the number of personal students.
    /// </summary>
    public required IReadOnlyDictionary<User, int> StudentsPerMentor { get; init; }
    
    public User? BackupMentor { get; init; }
    
    /// <summary>
    /// The general website settings
    /// </summary>
    public required Settings WebsiteSettings { get; init; }
    
    /// <summary>
    /// The optimal age difference between student and mentor
    /// </summary>
    public byte OptimalAgeDifference => WebsiteSettings.OptimalAgeDifference;

    /// <summary>
    /// A value between 0 and zero indicating the importance of availability in the determine which mentor is most suitable
    /// </summary>
    public double RelativeAvailabilityFactor => WebsiteSettings.RelativeAvailabilityFactor;
    
    /// <summary>
    /// A value between 0 and zero indicating the importance of age in the determine which mentor is most suitable
    /// </summary>
    public double RelativeAgeFactor => WebsiteSettings.RelativeAgeFactor;


    /// <summary>
    /// Holds if a strict gender policy should be applied.
    /// If this is true, students will only be connected with mentors of the same gender
    /// </summary>
    public bool StrictGender => WebsiteSettings.StrictGenderAssignment;
}