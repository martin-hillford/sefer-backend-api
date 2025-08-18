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
        var answers = await context.Answers.Where(a => a.SubmissionId == request.SubmissionId).ToListAsync(token);
        
        // Check the number of questions and answers if either one of them is zero, no grade can be calculated
        var totalQuestions = lesson.Content.Count(c => c.IsQuestion);
        if(totalQuestions == 0 || answers.Count == 0) return null;
        
        // The total number of questions and the number of answers should match
        if(totalQuestions != answers.Count) return null;
        
        // SEF-98 - Assign grade 10 if all questions are open
        var ungradableQuestions = lesson.Content
            .Where(c => c.Type == ContentBlockTypes.QuestionOpen)
            .Cast<OpenQuestion>().Where(c => !c.IsGradable).ToDictionary(c => c.Id);
        if(totalQuestions == ungradableQuestions.Count) return 1;

        // Bookkeeping for the grading - ungradable questions do not count towards the grade 
        var gradableQuestions = totalQuestions - ungradableQuestions.Count;

        // Check if for each answer a question can be found and create pairs
        var boolAnswers = answers.Where(a => a.QuestionType == ContentBlockTypes.QuestionBoolean).ToDictionary(a => a.QuestionId);
        var correctBoolAnswers = lesson.Content
            .Where(c => c.Type == ContentBlockTypes.QuestionBoolean && boolAnswers.ContainsKey(c.Id))
            .Count(c => boolAnswers[c.Id].IsCorrectBoolQuestion(c));

        var multipleChoiceAnswers = answers.Where(a => a.QuestionType == ContentBlockTypes.QuestionMultipleChoice).ToDictionary(a => a.QuestionId);
        var correctMultipleChoiceAnswers = lesson.Content
            .Where(c => c.Type == ContentBlockTypes.QuestionMultipleChoice && multipleChoiceAnswers.ContainsKey(c.Id))
            .Count(c => multipleChoiceAnswers[c.Id].IsCorrectMultipleChoiceQuestion(c));

        var openAnswers = answers.Where(a => a.QuestionType == ContentBlockTypes.QuestionOpen).ToDictionary(a => a.QuestionId);
        var correctExactAnswers = lesson.Content
            .Where(c => c.Type == ContentBlockTypes.QuestionOpen && openAnswers.ContainsKey(c.Id) )
            .Cast<OpenQuestion>()
            .Count(c => c.ExactAnswer != null && openAnswers[c.Id].TextAnswer.Trim().ToLower() == c.ExactAnswer?.ToLower().Trim());

        // Save the grade of the submission
        var totalCorrect = correctBoolAnswers + correctMultipleChoiceAnswers + correctExactAnswers;
        return totalCorrect / (float)gradableQuestions;
    }

}