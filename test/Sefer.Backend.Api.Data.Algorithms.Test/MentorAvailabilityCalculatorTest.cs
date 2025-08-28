namespace Sefer.Backend.Api.Data.Algorithms.Test;

[TestClass]
public class MentorAvailabilityCalculatorTest
{
    [TestMethod]
    public void Test_GetPreferredAvailabilityScores_SameAge()
    {
        var mentors = new List<MentorTestData>
        {
            new(1, 8, 10, 20, 1980), new(2, 8, 12, 20, 1980),
            new(3, 9, 12, 20, 1980), new(4, 9, 11, 20, 1980),
            new(5, 9, 8, 20, 1980),  new(6, 9, 9, 20, 1980),
        };
        var input = Create(mentors, 1980, 0.5);

        var scores = MentorAvailabilityCalculator.GetScores(input);
        var mentor = scores.GetMentorGivenPreferredScore(true);

        mentor.Should().NotBeNull();
        mentor.Id.Should().Be(2);
    }

    [TestMethod]
    public void Test_GetMentorGivenMaximumScore_SameAge()
    {
        var mentors = new List<MentorTestData>
        {
            new(1, 12, 10, 20, 1980), new(2, 14, 12, 20, 1980),
            new(3, 11, 12, 20, 1980), new(4, 9, 11, 20, 1980),
            new(5, 7, 8, 20, 1980),  new(6, 9, 9, 20, 1980),
            new(7, 21, 8, 20, 1980),  new(8, 28, 9, 20, 1980),
        };
        var input = Create(mentors, 1980, 0.5);

        var scores = MentorAvailabilityCalculator.GetScores(input);
        var mentor = scores.GetMentorGivenMaximumScore(true);

        mentor.Should().NotBeNull();
        // NB: 5 should not be found because that mentor has space in the preferred department
        mentor.Id.Should().Be(6);
    }

    [TestMethod]
    public void Test_GetPreferredAvailabilityScores_SameAvailabilityDifferentAge()
    {
        var mentors = new List<MentorTestData>
        {
            new(1, 8, 10, 20, 1969),
            new(2, 8, 10, 20, 1971),
            new(3, 8, 10, 20, 1970)
        };

        var input = Create(mentors, 1980, 0.5);
        var scores = MentorAvailabilityCalculator.GetScores(input);
        var mentor = scores.GetMentorGivenPreferredScore(true);

        // NB: Mentor 3 should be select since he has the optimal age diff of 0 years
        mentor.Should().NotBeNull();
        mentor.Id.Should().Be(3);
    }

    [TestMethod]
    public void Test_GetPreferredAvailabilityScores_NoGenderMatch()
    {
        var mentors = new List<MentorTestData> { new(1, 8, 10, 20, 1969), };

        var input = Create(mentors, 1980, 0.5);
        input.Student.Gender = Genders.Female;
        var scores = MentorAvailabilityCalculator.GetScores(input);
        var mentor = scores.GetMentorGivenPreferredScore(true);

        // NB: Mentor 3 should be select since he has the optimal age diff of 0 years
        Assert.IsNull(mentor);
    }

    private static MentorAssigningInput Create(List<MentorTestData> mentors, short studentYear, double ageFactor)
    {
        var users = new Dictionary<int, User>();
        var active = new Dictionary<int, int>();

        foreach (var mentor in mentors)
        {
            var mentorSettings = new MentorSettings { PreferredStudents = mentor.PreferredStudents, MaximumStudents = mentor.MaximumStudents };
            var user = new User { Id = mentor.Id, MentorSettings = mentorSettings, YearOfBirth = mentor.Year };
            users.Add(mentor.Id, user);
            active.Add(mentor.Id, mentor.ActiveStudents);
        }

        var student = new User { YearOfBirth = studentYear };
        var websiteSettings = new Settings { RelativeAgeFactor = ageFactor, OptimalAgeDifference = 10 };

        return new MentorAssigningInput
        {
            Mentors = users,
            Student = student,
            ActiveStudents = new MentorActiveStudentsDictionary(active),
            WebsiteSettings = websiteSettings
        };
    }
}