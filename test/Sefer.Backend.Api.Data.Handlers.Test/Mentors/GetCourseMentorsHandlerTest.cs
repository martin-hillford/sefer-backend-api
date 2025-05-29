namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class GetCourseMentorsHandlerTest : MentorUnitTest
{
    [TestInitialize]
    public override async Task Initialize()
    {
        await base.Initialize();

        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        var course2 = new Course { Name = "course.2", Description = "course.2", Permalink = "course1" };
        var course3 = new Course { Name = "course.3", Description = "course.3", Permalink = "course1" };
        await InsertAsync(course1, course2, course3);
    }

    [TestMethod]
    public async Task Handle_NoMentor()
    {
        var context = GetDataContext();
        var course1 = context.Courses.Single(c => c.Name == "course.1");

        var mentors = await Handle(course1.Id);

        mentors.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_NoCourses()
    {
        var context = GetDataContext();
        var course1 = context.Courses.Single(c => c.Name == "course.1");
        var course2 = context.Courses.Single(c => c.Name == "course.2");
        var mentor = await context.GetMentor();
        await InsertAsync(new MentorCourse { CourseId = course1.Id, MentorId = mentor.Id });

        var mentors = await Handle(course2.Id);

        mentors.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var course1 = context.Courses.Single(c => c.Name == "course.1");
        var mentor = await context.GetMentor();
        await InsertAsync(new MentorCourse { CourseId = course1.Id, MentorId = mentor.Id });

        var mentors = await Handle(course1.Id);

        mentors.Count.Should().Be(1);
    }

    private async Task<List<User>> Handle(int courseId)
    {
        var request = new GetCourseMentorsRequest(courseId);
        var handler = new GetCourseMentorsHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}