namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class GetBackupMentorHandlerTest : MentorUnitTest
{
    [TestMethod]
    public async Task Handle_NoSettings()
    {
        var provider = GetServiceProvider().Object;
        var handler = new GetBackupMentorHandler(provider);
        var request = new GetBackupMentorRequest();

        var backup = await handler.Handle(request, CancellationToken.None);
        
        Assert.IsNull(backup);
    }

    [TestMethod]
    public async Task Handle_NoBackupMentor()
    {
        var backup = await Handle(null);
        Assert.IsNull(backup);
    }

    [TestMethod]
    public async Task Handle_UserIsMentor()
    {
        var context = GetDataContext();
        var student = await context.GetStudent();
        var backup = await Handle(student);
        Assert.IsNull(backup);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var backup = await Handle(mentor);
        Assert.AreEqual(mentor.Id, backup?.Id);
    }

    private async Task<User?> Handle(User? backup)
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var settings = new WebsiteSettings { BackupMentorId = mentor.Id };
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetSettingsRequest, WebsiteSettings>(settings);
        if(backup != null) provider.AddRequestResult<GetUserByIdRequest, User>(backup);

        var handler = new GetBackupMentorHandler(provider.Object);
        var request = new GetBackupMentorRequest();

        return await handler.Handle(request, CancellationToken.None);
    }
}