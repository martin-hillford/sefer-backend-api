namespace Sefer.Backend.Api.Data.Handlers.Test.Surveys;

[TestClass]
public class AddSurveyResultHandlerTest : AddEntityHandlerTest<AddSurveyResultRequest, AddSurveyResultHandler, SurveyResult>
{
    [TestInitialize]
    public async Task Initialize()
    {
        var course = new Course {Name = "course.1", Description = "course.1", Permalink = "course1" };
        await InsertAsync(course);
        
        var courseRevision = new CourseRevision {CourseId = course.Id, Stage = Stages.Edit, Version = 1};
        await InsertAsync(courseRevision);
        
        var student = new User { Name = "Student", Role = UserRoles.Student, Gender = Genders.Male, Email = "student@example.tld", YearOfBirth = 1987 };
        var mentor = new User { Name = "Mentor", Role = UserRoles.Student, Gender = Genders.Male, Email = "mentor@example.tld", YearOfBirth = 1987 };
        await InsertAsync(student, mentor);

        var enrollment = new Enrollment { MentorId = mentor.Id, StudentId = student.Id, CourseRevisionId = courseRevision.Id};
        await InsertAsync(enrollment);
    }
    
    protected override List<(SurveyResult Entity, bool Valid)> GetTestData()
    {
        return
        [
            (new SurveyResult { Text = "text", EnrollmentId = 1 }, true)
        ];
    }
}