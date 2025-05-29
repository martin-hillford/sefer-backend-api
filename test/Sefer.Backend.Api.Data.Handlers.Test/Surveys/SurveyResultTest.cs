namespace Sefer.Backend.Api.Data.Handlers.Test.Surveys;

public abstract class SurveyResultTest : HandlerUnitTest
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

        var courseRating = new CourseRating {CourseId = course.Id, Rating = 3, CreationDate = DateTime.UtcNow};
        await InsertAsync(courseRating);
        
        var mentorRating = new MentorRating { MentorId = mentor.Id, Rating = 4, CreationDate = DateTime.UtcNow};
        await InsertAsync(mentorRating);

        var result1 = new SurveyResult
        {
            EnrollmentId = enrollment.Id, 
            Text = "text1", 
            SocialPermissions = true, 
            CourseRatingId = courseRating.Id,
            MentorRatingId = mentorRating.Id,
            AdminProcessed = true
        };
        
        var result2 = new SurveyResult
        {
            EnrollmentId = enrollment.Id, 
            Text = "text2", 
            SocialPermissions = true, 
            CourseRatingId = courseRating.Id,
            MentorRatingId = mentorRating.Id,
        };
        
        await InsertAsync(result1, result2);
    }
}