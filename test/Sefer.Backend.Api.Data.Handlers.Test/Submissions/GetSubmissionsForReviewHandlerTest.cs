namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class GetSubmissionsForReviewHandlerTest : GetSubmissionsForReviewCountHandlerTest
{
    [TestMethod]
    public override async Task Handle_MentorNull()
    {
        var count = await Handle(13);
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public virtual async Task Handle_NoSubmissionDate()
    {
        var mentor = await GetMentor();
        await PrepareSubmission(true, false, null);
        var count = await Handle(mentor.Id);
        Assert.AreEqual(0, count);
    }

    protected override async Task<long?> Handle(int mentorId)
    {
        var context = GetDataContext();
        var mentor = context.Users.SingleOrDefault(u => u.Id == mentorId);

        var provider = GetServiceProvider();
        if(mentor != null) provider.AddRequestResult<GetUserByIdRequest, User>(mentor);

        var request = new GetSubmissionsForReviewRequest(mentorId);
        var handler = new GetSubmissionsForReviewHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        return result.Count;
    }
}