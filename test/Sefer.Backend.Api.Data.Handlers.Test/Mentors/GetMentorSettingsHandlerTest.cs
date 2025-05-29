namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class GetMentorSettingsHandlerTest : MentorUnitTest
{
    [TestMethod]
    public async Task Handle_SettingsNull()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var settings = await Handle(mentor.Id);
        settings.Should().BeNull();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        await InsertAsync(new MentorSettings { MentorId = mentor.Id });
        var settings = await Handle(mentor.Id);
        settings.Should().NotBeNull();
    }

    private async Task<MentorSettings> Handle(int mentorId)
    {
        var request = new GetMentorSettingsRequest { MentorId = mentorId };
        var handler = new GetMentorSettingsHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}