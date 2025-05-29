namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class AddMentorRatingHandlerTest : MentorUnitTest
{
    [TestMethod]
    public async Task Handle_RatingNull()
    {
        var added = await Handle(null);
        added.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle_RatingTooHigh()
    {
        var rating = new MentorRating { Rating = 11 };
        var added = await Handle(rating);
        added.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle_RatingHasId()
    {
        var rating = new MentorRating { Rating = 9, Id = 3 };
        var added = await Handle(rating);
        added.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle_MentorNotFound()
    {
        var rating = new MentorRating { Rating = 9, MentorId = 11 };
        var added = await Handle(rating);
        added.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle_UserNotMentor()
    {
        var context = GetDataContext();
        var student = await context.GetStudent();
        var rating = new MentorRating { Rating = 9, MentorId = student.Id };
        var added = await Handle(rating);
        added.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var rating = new MentorRating { Rating = 9, MentorId = mentor.Id };
        var added = await Handle(rating);
        added.Should().BeTrue();

        context.MentorRatings.Count().Should().Be(1);
    }

    private async Task<bool> Handle(MentorRating? rating)
    {

        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var user = rating?.MentorId == mentor.Id ? mentor : null;
        var provider = GetServiceProvider();
        if(user != null) provider.AddRequestResult<GetUserByIdRequest, User>(user);

        var request = new AddMentorRatingRequest(rating);
        var handler = new AddMentorRatingHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}