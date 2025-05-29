namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetSubmissionForReviewByIdHandler(IServiceProvider serviceProvider)
    : Handler<GetSubmissionForReviewByIdRequest, LessonSubmission>(serviceProvider)
{
    public override async Task<LessonSubmission> Handle(GetSubmissionForReviewByIdRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var submission = await context.LessonSubmissions
            .Where(s => s.Id == request.SubmissionId && s.Enrollment.MentorId == request.MentorId && s.IsFinal && !s.ResultsStudentVisible)
            .Include(s => s.Enrollment).ThenInclude(e => e.Student)
            .Include(s => s.Enrollment).ThenInclude(e => e.Mentor)
            .Include(s => s.Enrollment).ThenInclude(e => e.CourseRevision).ThenInclude(c => c.Course)
            .Include(s => s.Answers)
            .FirstOrDefaultAsync(token);
        if (submission == null || submission.Imported) return null;
        submission.Lesson = await Send(new GetLessonIncludeReferencesRequest(submission.LessonId), token);

        // Because of an issue in the past, sometimes it is not calculated if the answer is correct
        var missing = CalculateCorrectAnswers(submission);
        if (missing) await context.SaveChangesAsync(token);

        return submission;
    }

    private static bool CalculateCorrectAnswers(LessonSubmission submission)
    {
        var unValidated = submission.Answers.Where(a => a.IsCorrectAnswer == null && a.QuestionType != ContentBlockTypes.QuestionOpen).ToList();
        foreach (var answer in unValidated)
        {
            var question = submission.Lesson.Content.SingleOrDefault(c => c.Type == answer.QuestionType && c.Id == answer.QuestionId);
            if (question == null) continue;
            switch (answer.QuestionType)
            {
                case ContentBlockTypes.QuestionBoolean:
                    answer.IsCorrectAnswer = answer.IsCorrectBoolQuestion(question);
                    break;
                case ContentBlockTypes.QuestionMultipleChoice:
                    answer.IsCorrectAnswer = answer.IsCorrectMultipleChoiceQuestion(question);
                    break;
            }
        }

        return unValidated.Count != 0;
    }
}