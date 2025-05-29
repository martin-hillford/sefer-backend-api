namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class CalculateSubmissionGradeHandler(IServiceProvider serviceProvider)
    : Handler<CalculateSubmissionGradeRequest, double?>(serviceProvider)
{
    public override async Task<double?> Handle(CalculateSubmissionGradeRequest request, CancellationToken token)
    {
        // First check if a submission can be found
        var submission = await Send(new GetSubmissionByIdRequest(request.SubmissionId), token);
        if (submission == null) throw new NullReferenceException();

        // Next get the enrollment, the lesson and the answers
        var enrollment = await Send(new GetEnrollmentByIdRequest(submission.EnrollmentId), token);
        if (enrollment == null) throw new NullReferenceException();

        var lesson = await Send(new GetLessonIncludeReferencesRequest(submission.LessonId), token);
        if (lesson == null) throw new NullReferenceException();

        var context = GetDataContext();
        var answers = await context.Answers
            .Where(a => a.SubmissionId == request.SubmissionId)
            .ToListAsync(token);

        // SEF-98 - Assign the grade 10 if all question are open
        var totalQuestions = lesson.Content.Count(c => c.IsQuestion);
        var totalOpenQuestions = lesson.Content.Count(c => c.Type == ContentBlockTypes.QuestionOpen);
        if (totalOpenQuestions == totalQuestions) return 1;

        // Bookkeeping for the grading
        var totalAnswersGiven = answers.Count(a => a.QuestionType != ContentBlockTypes.QuestionOpen);

        // Check if for each answer a question can be found and create pairs
        var pairs = answers
            .Select(a => (lesson.Content.First(c => c.Type == a.QuestionType && c.Id == a.QuestionId), a))
            .ToList<(IContentBlock<Lesson> Block, QuestionAnswer Answer)>();

        var correctBoolAnswers = pairs
            .Where(a => a.Answer.QuestionType == ContentBlockTypes.QuestionBoolean)
            .Count(p => p.Answer.IsCorrectBoolQuestion(p.Block));

        var correctMultipleChoiceAnswers = pairs
            .Where(a => a.Answer.QuestionType == ContentBlockTypes.QuestionMultipleChoice)
            .Count(p => p.Answer.IsCorrectMultipleChoiceQuestion(p.Block));

        // Save the grade of the submission
        if (totalAnswersGiven != 0) return (correctBoolAnswers + correctMultipleChoiceAnswers) / (float)totalAnswersGiven;

        // done
        return null;
    }

}