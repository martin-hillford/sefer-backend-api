namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// This class is a dictionary for easy lookup of the number of active students of a mentor
/// </summary>
public class MentorActiveStudentsDictionary(IDictionary<int, int> data)
{
    public int GetActiveStudents(int mentorId)
    {
        return data.TryGetValue(mentorId, out var value) ? value : 0;
    }
}
