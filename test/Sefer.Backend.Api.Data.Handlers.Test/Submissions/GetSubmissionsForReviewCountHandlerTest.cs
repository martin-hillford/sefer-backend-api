namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class GetSubmissionsForReviewCountHandlerTest : SubmissionUnitTest
{
    [TestMethod]
    public virtual async Task Handle_MentorNull()
    {
        var count = await Handle(13);
        Assert.IsNull(count);
    }

    [TestMethod]
    public virtual async Task Handle_NotMentorOfSubmission()
    {
        await PrepareSubmission(true, false, DateTime.Now);
        var context = GetDataContext();
        var student = context.Users.Single(s => s.Role == UserRoles.Student);
        var count = await Handle(student.Id);
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public virtual async Task Handle_SubmissionNotFinal()
    {
        var mentor = await GetMentor();
        await PrepareSubmission(false, false, DateTime.Now);
        var count = await Handle(mentor.Id);
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public virtual async Task Handle_StudentVisible()
    {
        var mentor = await GetMentor();
        await PrepareSubmission(true, true, DateTime.Now);
        var count = await Handle(mentor.Id);
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public virtual async Task Handle()
    {
        var mentor = await GetMentor();
        await PrepareSubmission(true, false, DateTime.Now);
        var count = await Handle(mentor.Id);
        Assert.AreEqual(1, count);
    }

    protected Task<User> GetMentor()
    {
        var context = GetDataContext();
        return Task.FromResult(context.Users.Single(s => s.Role == UserRoles.Mentor));
    }

    protected async Task PrepareSubmission(bool isFinal, bool resultsStudentVisible, DateTime? submissionDate)
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var lesson = context.Lessons.Single();
        var submission = new LessonSubmission
        {
            EnrollmentId = enrollment.Id,
            IsFinal = isFinal,
            LessonId = lesson.Id,
            ResultsStudentVisible = resultsStudentVisible,
            SubmissionDate = submissionDate
        };
        await InsertAsync(submission);
    }

    protected virtual async Task<long?> Handle(int mentorId)
    {
        var context = GetDataContext();
        var mentor = context.Users.SingleOrDefault(u => u.Id == mentorId);

        var provider = GetServiceProvider();
        if(mentor != null) provider.AddRequestResult<GetUserByIdRequest, User>(mentor);

        var request = new GetSubmissionsForReviewCountRequest(mentorId);
        var handler = new GetSubmissionsForReviewCountHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}