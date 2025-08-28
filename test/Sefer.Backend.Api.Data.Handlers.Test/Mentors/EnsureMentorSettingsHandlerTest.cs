

namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class EnsureMentorSettingsHandlerTest : MentorUnitTest
{
    [TestMethod]
    public async Task Handle_MentorNull()
    {
        var settings = await Handle(null);
        Assert.IsNull(settings);
    }

    [TestMethod]
    public async Task Handle_UserIsNoMentor()
    {
        var context = GetDataContext();
        var student = await context.GetStudent();
        var settings = await Handle(student);
        Assert.IsNull(settings);
    }

    [TestMethod]
    public async Task Handle_ExistingSettings()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        await InsertAsync(new MentorSettings { MentorId = mentor.Id });
        var settings = await Handle(mentor);
        Assert.IsNotNull(settings);
        await VerifyMentorSettings();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var settings = await Handle(mentor);
        Assert.IsNotNull(settings);
        await VerifyMentorSettings();
    }

    private async Task<MentorSettings> Handle(User? user)
    {
        var site = new Mock<ISite>();
        site.SetupGet(s => s.RegionId).Returns("nl");
        site.SetupGet(s => s.Hostname).Returns("test.tld");

        var region = new Mock<IRegion>();
        region.SetupGet(s => s.Id).Returns("nl");
        
        var provider = GetServiceProvider()
            .AddRequestResult<GetUserByIdRequest, User?>(user)
            .AddRequestResult<GetSiteByNameRequest, ISite?>(site.Object)
            .AddRequestResult<GetPrimaryRegionAndSiteRequest, (IRegion, ISite)>((region.Object, site.Object));

        var request = new EnsureMentorSettingsRequest(user?.Id ?? 0);
        var handler = new EnsureMentorSettingsHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }

    private async Task VerifyMentorSettings()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        Assert.IsNotNull(mentor);

        var settings = await context.MentorSettings.SingleOrDefaultAsync();
        Assert.IsNotNull(settings);
    }
}