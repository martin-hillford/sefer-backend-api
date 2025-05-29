namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class SaveSubmissionRequest(LessonSubmission submission, List<QuestionAnswer> answers) : IRequest<bool>
{
    public readonly LessonSubmission Submission = submission;

    public readonly List<QuestionAnswer> Answers = answers;
}