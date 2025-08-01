namespace Sefer.Backend.Api.Data.Algorithms.Test;

internal class PersonalMentorAssigningBuilder(Genders studentGender, short studentYearOfBirth)
{
    private readonly List<User> _mentors = [];

    private readonly Dictionary<User, int> _count = [];
    
    private User? _backupMentor;
    
    public PersonalMentorAssigningBuilder AddMentor(int mentorId, short yearOfBirth, int personalStudents, short preferredStudents, short maximumStudents, Genders gender)
    {
        var settings = new MentorSettings { MentorId = mentorId, MaximumStudents = maximumStudents, PreferredStudents = preferredStudents };
        var mentor = new User { Id = mentorId, YearOfBirth = yearOfBirth, Gender = gender, Role = UserRoles.Mentor,  MentorSettings = settings };
        _mentors.Add(mentor);
        _count.Add(mentor, personalStudents);
        return this;
    }

    public void AddBackupMentor(int mentorId, Genders gender)
    {
        _backupMentor = new User { Id = mentorId, Gender = gender, Role = UserRoles.Mentor };
    }

    public PersonalMentorAssigning GetAssigner(bool strictGender, byte optimalAgeDifference, double relativeAgeFactor)
    {
        var student = new User { Id = 1, YearOfBirth = studentYearOfBirth, Gender = studentGender };
        var settings = new Settings
        {
            OptimalAgeDifference = optimalAgeDifference, RelativeAgeFactor = relativeAgeFactor,
            StrictGenderAssignment = strictGender
        };
        var input = new PersonalMentorAssigningInput
        {
            Mentors = _mentors,
            Student = student,
            BackupMentor = _backupMentor,
            WebsiteSettings = settings,
            StudentsPerMentor = _count
        };
        return new PersonalMentorAssigning(input);
    }
}