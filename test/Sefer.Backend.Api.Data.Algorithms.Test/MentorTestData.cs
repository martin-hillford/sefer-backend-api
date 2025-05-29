namespace Sefer.Backend.Api.Data.Algorithms.Test;

public class MentorTestData(int id, int active, short preferred, short maximum, short year)
{
    public readonly int Id = id;

    public readonly int ActiveStudents = active;

    public readonly short PreferredStudents = preferred;

    public readonly short MaximumStudents = maximum;

    public readonly short Year = year;
}
