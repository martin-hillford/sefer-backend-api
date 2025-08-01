namespace Sefer.Backend.Api.Data.Algorithms;

public readonly struct MentorAssigningInput
{
    /// <summary>
    /// The student to which a mentor needs to be assigned
    /// </summary>
    public User Student { get; init; }

    /// <summary>
    /// The dictionary of mentors that are teaching the courses that the student wants to take.
    /// Please ensure the MentorSettings are loaded for each mentor
    /// </summary>
    /// <remarks>The key of the dictionary is the id of the mentor</remarks>
    public IReadOnlyDictionary<int, User> Mentors { get; init; }

    /// <summary>
    /// A dictionary that contains for each mentor the number of active students
    /// </summary>
    public MentorActiveStudentsDictionary ActiveStudents { get; init; }

    /// <summary>
    /// The last enrollment of the student
    /// </summary>
    public Enrollment? LastStudentEnrollment { get; init; }

    /// <summary>
    /// The general website settings
    /// </summary>
    public Settings WebsiteSettings { get; init; }

    /// <summary>
    /// if no other mentor is available, this is the backup mentor
    /// </summary>
    public User BackupMentor { get; init; }

    /// <summary>
    /// The number of days that may between now and the last enrollment of the student that give the right to maintain the same mentor (if possible given the course)
    /// </summary>
    public short SameMentorDays => WebsiteSettings.SameMentorDays;

    /// <summary>
    /// The optimal age difference between student and mentor
    /// </summary>
    public byte OptimalAgeDifference => WebsiteSettings.OptimalAgeDifference;

    /// <summary>
    /// A value between 0 and zero indicating the importance of age in the determine which mentor is most suitable
    /// </summary>
    public double RelativeAgeFactor => WebsiteSettings.RelativeAgeFactor;

    /// <summary>
    /// A value between 0 and zero indicating the importance of availability in the determine which mentor is most suitable
    /// </summary>
    public double RelativeAvailabilityFactor => 1 - RelativeAgeFactor;
}