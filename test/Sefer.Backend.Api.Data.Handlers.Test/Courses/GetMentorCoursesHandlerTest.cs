namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetMentorCoursesHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_NoUser()
    {
        await Handle(null, 1, new List<int>());
    }
    
    [TestMethod]
    public async Task Handle_NoMentor()
    {
        await Handle(new User { Role = UserRoles.Student }, 1, new List<int>());
    }
    
    [TestMethod]
    public async Task Handle_NoCourses()
    {
        await Handle(new User { Role = UserRoles.Mentor }, 1, new List<int>());
    }
    
    [TestMethod]
    public async Task Handle()
    {
        var course1 = new Course {Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true};
        var course2 = new Course {Name = "course.2", Description = "course.2", Permalink = "course2"};
        var course3 = new Course {Name = "course.3", Description = "course.3", Permalink = "course3", ShowOnHomepage = true};
        await InsertAsync(course1, course2, course3);
        
        var mentor1 = new User { Role = UserRoles.Mentor, Name = "a", Gender = Genders.Male, Email = "a@e.tld", YearOfBirth = 1987};
        var mentor2 = new User { Role = UserRoles.Mentor, Name = "b", Gender = Genders.Male, Email = "b@e.tld", YearOfBirth = 1987};
        await InsertAsync(mentor1, mentor2);

        await InsertAsync
        (
            new MentorCourse {CourseId = course2.Id, MentorId = mentor1.Id},
            new MentorCourse {CourseId = course2.Id, MentorId = mentor2.Id},
            new MentorCourse {CourseId = course3.Id, MentorId = mentor2.Id}
        );

        var courseIds = new List<int> {course2.Id, course3.Id};
        await Handle(mentor2, mentor2.Id, courseIds);
    }

    private async Task Handle(User? mentor, int mentorId, IEnumerable<int> courseIds)
    {
        var provider = GetServiceProvider();
        if(mentor != null) provider.AddRequestResult<GetUserByIdRequest, User>(mentor);

        var request = new GetMentorCoursesRequest(mentorId);
        var handler = new GetMentorCoursesHandler(provider.Object);
        var courses = await handler.Handle(request, CancellationToken.None);
        
        Assert.AreEqual(courseIds.Count(), courses.Count);
        Assert.IsTrue(courses.All(c => courses.Contains(c)));
    }
}