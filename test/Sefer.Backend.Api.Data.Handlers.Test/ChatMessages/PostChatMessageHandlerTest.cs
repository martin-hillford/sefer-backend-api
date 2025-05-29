namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

public abstract class PostChatMessageHandlerTest : ChatMessageUnitTest
{
    protected async Task<(Channel channel, LessonSubmission submission)> InitializeSubmission(bool resultsStudentVisible, string? review = null)
    {
        var (channel, student, mentor) = await InitializePersonalChannel();
        var context = GetDataContext();
        await SubmissionUnitTest.InitializeSubmission(context, student, mentor);

        if (!string.IsNullOrEmpty(review))
        {
            var answer = context.Answers.First();
            answer.MentorReview = review;
            context.UpdateSingleProperty(answer, nameof(answer.MentorReview));
        }

        var submission = await context.LessonSubmissions
            .Include(s => s.Enrollment).ThenInclude(e => e.Student)
            .Include(s => s.Enrollment).ThenInclude(e => e.Mentor)
            .Include(s => s.Enrollment).ThenInclude(e => e.CourseRevision).ThenInclude(c => c.Course)
            .Include(s => s.Answers)
            .SingleAsync();

        submission.ResultsStudentVisible = resultsStudentVisible;
        context.UpdateSingleProperty(submission, nameof(submission.ResultsStudentVisible));

        var lesson = await context.Lessons.Include(l => l.CourseRevision).ThenInclude(c => c.Course).SingleAsync();
        var question = await context.LessonBoolQuestions.SingleAsync();
        lesson.Content = new List<IContentBlock<Lesson>> { question };
        submission.Lesson = lesson;

        return (channel, submission);
    }
}