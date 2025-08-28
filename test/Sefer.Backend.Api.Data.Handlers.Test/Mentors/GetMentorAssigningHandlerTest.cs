namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class GetMentorAssigningHandlerTest : MentorUnitTest
{
    [TestMethod]
    public async Task Handle_SettingsNull()
    {
        var assigned = await Handle(null, null, null, null);
        Assert.IsNull(assigned);
    }

    [TestMethod]
    public async Task Handle_BackupMentorNull()
    {
        var settings = new WebsiteSettings();
        var assigned = await Handle(settings, null, null, null);
        Assert.IsNull(assigned);
    }

    [TestMethod]
    public async Task Handle_BackupIsNotMentor()
    {
        var context = GetDataContext();
        var student = await context.GetStudent();
        var settings = new WebsiteSettings();
        var assigned = await Handle(settings, student, null, null);
        Assert.IsNull(assigned);
    }

    [TestMethod]
    public async Task Handle_StudentNull()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var settings = new WebsiteSettings();
        var assigned = await Handle(settings, mentor, null, null);
        Assert.IsNull(assigned);
    }

    [TestMethod]
    public async Task Handle_CourseNull()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var student = await context.GetStudent();
        var settings = new WebsiteSettings();
        var assigned = await Handle(settings, mentor, student, null);
        Assert.IsNull(assigned);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var student = await context.GetStudent();
        var settings = new WebsiteSettings();
        var course = new Course();
        var assigned = await Handle(settings, mentor, student, course);
        Assert.AreEqual(mentor, assigned);
    }

    [TestMethod]
    public async Task Handle_Exception()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var settings = new WebsiteSettings();

        var provider = GetServiceProvider()
            .AddRequestResult<GetBackupMentorRequest, User?>(mentor)
            .AddRequestResult<GetSettingsRequest, WebsiteSettings?>(settings)
            .AddRequestException<GetUserByIdRequest, User>();

        var assigned = await Handle(provider);
        Assert.AreEqual(mentor, assigned);
    }

    [TestMethod]
    public async Task Handle_PersonalMentor()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();

        var provider = GetServiceProvider()
            .AddRequestResult<GetPersonalMentorForCourseRequest, User?>(mentor);

        var assigned = await Handle(provider);
        Assert.AreEqual(mentor, assigned);
    }


    private async Task<User> Handle(WebsiteSettings? settings, User? mentor, User? student, Course? course)
    {
        var factory = new Mock<IMentorAssigningFactory>();
        var algo = new Mock<IMentorAssigningAlgorithm>();

        if(mentor != null) algo.Setup(c => c.GetMentor()).Returns(mentor);
        factory.Setup(f => f.PrepareAlgorithm(It.IsAny<MentorAssigningInput>())).Returns(algo.Object);

        var provider = GetServiceProvider()
            .AddRequestResult<GetBackupMentorRequest, User?>(mentor)
            .AddRequestResult<GetSettingsRequest, WebsiteSettings?>(settings)
            .AddRequestResult<GetUserByIdRequest, User?>(student)
            .AddRequestResult<GetPublishedCourseByIdRequest, Course?>(course)
            .AddService<IMentorAssigningFactory, IMentorAssigningFactory>(factory.Object);

        if (mentor != null) provider.AddRequestResult<GetMentorsOfCourseRequest, List<User>>([mentor]);
        return await Handle(provider);
    }

    private static async Task<User> Handle(MockedServiceProvider provider)
    {
        var request = new GetMentorAssigningRequest(9, 8);
        var handler = new GetMentorAssigningHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}