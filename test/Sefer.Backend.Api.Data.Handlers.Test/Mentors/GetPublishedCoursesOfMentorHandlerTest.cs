namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class GetPublishedCoursesOfMentorHandlerTest : MentorUnitTest
{
    [TestMethod]
    public async Task Handle_MentorNull()
    {
        var courses = await Handle(1, null);
        courses.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_UserIsNoMentor()
    {
        var context = GetDataContext();
        var student = await context.GetStudent();
        var courses = await Handle(student.Id, student);
        courses.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_NoPublishedCourse()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var courses = await Handle(mentor.Id, mentor);
        courses.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var course = new Course { Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true };
        await InsertAsync(course);
        await InsertAsync(new CourseRevision { CourseId = course.Id, Stage = Stages.Published, Version = 1 });
        await InsertAsync(new MentorCourse { CourseId = course.Id, MentorId = mentor.Id });

        var courses = await Handle(mentor.Id, mentor);

        courses.Should().NotBeEmpty();
        courses.First().Id.Should().Be(course.Id);
    }

    private async Task<List<CourseRevision>> Handle(int mentorId, User? user)
    {
        var request = new GetPublishedCoursesOfMentorRequest(mentorId);
        var provider = GetServiceProvider();
        if(user != null ) provider.AddRequestResult<GetUserByIdRequest, User>(user);
        var handler = new GetPublishedCoursesOfMentorHandler(provider.Object);
        return await handler.Handle(request,CancellationToken.None);
    }
}