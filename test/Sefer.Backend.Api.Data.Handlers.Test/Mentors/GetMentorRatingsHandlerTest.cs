namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class GetMentorRatingsHandlerTest : MentorUnitTest
{
    [TestMethod]
    public async Task Handle_StudentRole()
    {
        await EnsureRating(5, 6, 7);
        var ratings = await Handle(UserRoles.Student);
        ratings.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_NoRatings()
    {
        var ratings = await Handle(UserRoles.Student);
        ratings.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();

        await EnsureRating(5, 6, 7);
        var ratings = await Handle(UserRoles.Mentor);
        ratings.Count.Should().Be(1);

        ratings[mentor.Id].Item1.Should().Be(6);
        ratings[mentor.Id].Item2.Should().Be(3);
    }

    private async Task EnsureRating(params byte[] ratings)
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        foreach (var rating in ratings)
        {
            await InsertAsync(new MentorRating { MentorId = mentor.Id, Rating = rating });
        }
    }

    private async Task<Dictionary<int, Tuple<int, int>>> Handle(UserRoles role)
    {
        var request = new GetMentorRatingsRequest(role);
        var handler = new GetMentorRatingsHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}