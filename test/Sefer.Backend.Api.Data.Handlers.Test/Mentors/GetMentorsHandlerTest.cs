namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class GetMentorsHandlerTest : MentorUnitTest
{
    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        await InsertAsync(new MentorSettings { MentorId = mentor.Id });

        var request = new GetMentorsRequest();
        var handler = new GetMentorsHandler(GetServiceProvider().Object);

        var mentors = await handler.Handle(request, CancellationToken.None);
        mentors.Count.Should().Be(1);
        mentors.First().Id.Should().Be(mentor.Id);
        mentors.First().MentorSettings.Should().NotBeNull();
    }
}