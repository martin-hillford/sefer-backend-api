namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class SetMentorForCourseRequestTest : MentorUnitTest
{
    [TestMethod]
    public async Task Handle_MentorNull()
    {
        var result = await Handle(null, null);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_UserIsNoMentor()
    {
        var context = GetDataContext();
        var student = await context.GetStudent();
        var result = await Handle(null, student);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_CourseNull()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var result = await Handle(null, mentor);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_MentorInvalidId()
    {
        var result = await Handle(null, new User { Id = -1 });
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_CourseInvalidId()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var result = await Handle(new Course { Id = -1 }, mentor);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_MentorAlreadyForCourse()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var course = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        await InsertAsync(course);
        await InsertAsync(new MentorCourse { MentorId = mentor.Id, CourseId = course.Id });

        context.MentorCourses.Count().Should().Be(1);

        var result = await Handle(course, mentor);

        Assert.IsTrue(result);
        context.MentorCourses.Count().Should().Be(1);
    }

    [TestMethod]
    public async Task Handle_Exception()
    {
        var provider = GetServiceProvider()
            .AddRequestException<GetCourseByIdRequest, Course>();

        var request = new SetMentorForCourseRequest(1, 1);
        var handler = new SetMentorForCourseHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var course = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        await InsertAsync(course);

        var result = await Handle(course, mentor);

        Assert.IsTrue(result);
        context.MentorCourses.Count().Should().Be(1);
    }

    private async Task<bool> Handle(Course? course, User? mentor)
    {
        var provider = GetServiceProvider()
            .AddRequestResult<GetCourseByIdRequest, Course?>(course)
            .AddRequestResult<GetUserByIdRequest, User?>(mentor);
        var request = new SetMentorForCourseRequest(course?.Id ?? 1, mentor?.Id ?? 1);
        var handler = new SetMentorForCourseHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}