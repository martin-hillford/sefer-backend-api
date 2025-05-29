namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// This class contains data from mentor about student
/// </summary>
public class MentorStudentData : Entity
{
    /// <summary>
    /// The id of the mentor that manages the data
    /// </summary>
    public int MentorId { get; set; }

    /// <summary>
    /// The id of the student the data is about
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// The remarks of the mentor about the student
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string Remarks { get; set; }
}