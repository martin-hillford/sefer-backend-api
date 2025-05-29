namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class GetMentorsWithSettingsHandlerTest : MentorUnitTest
{
    [TestMethod]
    [DataRow(UserRoles.User, 0)]
    [DataRow(UserRoles.Student, 0)]
    [DataRow(UserRoles.Mentor, 1)]
    [DataRow(UserRoles.CourseMaker, 0)]
    [DataRow(UserRoles.Admin, 0)]
    public async Task Handle(UserRoles role, int count)
    {
        var request = new GetMentorsWithSettingsRequest(role);
        var handler = new GetMentorsWithSettingsHandler(GetServiceProvider().Object);
        var users = await handler.Handle(request, CancellationToken.None);
        users.Count.Should().Be(count);
    }
}