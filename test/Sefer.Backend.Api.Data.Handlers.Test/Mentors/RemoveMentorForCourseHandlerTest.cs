namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class RemoveMentorForCourseHandlerTest : MentorUnitTest
{
    [TestMethod]
    public async Task Handle_MentorNull()
    {
        var result = await Handle(null, null);
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle_UserIsNoMentor()
    {
        var context = GetDataContext();
        var student = await context.GetStudent();
        var result = await Handle(null, student);
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle_CourseNull()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var result = await Handle(null, mentor);
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle_MentorInvalidId()
    {
        var result = await Handle(null, new User { Id = -1 });
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle_CourseInvalidId()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var result = await Handle(new Course { Id = -1 }, mentor);
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle_MentorAlreadyNotForCourse()
    {
        var course = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        await InsertAsync(course);

        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var result = await Handle(course, mentor);
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task Handle_Exception()
    {
        var provider = GetServiceProvider()
            .AddRequestException<GetCourseByIdRequest, Course>();

        var request = new RemoveMentorForCourseRequest(1, 1);
        var handler = new RemoveMentorForCourseHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var course = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        await InsertAsync(course);
        await InsertAsync(new MentorCourse { MentorId = mentor.Id, CourseId = course.Id });

        context.MentorCourses.Count().Should().Be(1);

        var result = await Handle(course, mentor);

        result.Should().BeTrue();
        context.MentorCourses.Count().Should().Be(0);
    }

    private async Task<bool> Handle(Course? course, User? mentor)
    {
        var provider = GetServiceProvider()
            .AddRequestResult<GetCourseByIdRequest, Course?>(course)
            .AddRequestResult<GetUserByIdRequest, User?>(mentor);
        var request = new RemoveMentorForCourseRequest(course?.Id ?? 1, mentor?.Id ?? 1);
        var handler = new RemoveMentorForCourseHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}