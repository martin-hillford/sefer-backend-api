namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class SaveSubmissionHandler(IServiceProvider serviceProvider)
    : Handler<SaveSubmissionRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(SaveSubmissionRequest request, CancellationToken token)
    {
        try
        {
            // First deal with the submission
            if (await IsValidAsync(request.Submission) == false) return false;

            request.Submission.ModificationDate = DateTime.UtcNow;
            if (request.Submission.IsFinal) request.Submission.SubmissionDate = DateTime.UtcNow;

            var context = GetAsyncContext();

            var saved = await context.InsertOrUpdate(request.Submission, token);
            if (saved == false) throw new SaveChangesException();

            // Deal with the answers
            foreach (var answer in request.Answers)
            {
                answer.ModificationDate = DateTime.UtcNow;
                answer.SubmissionId = request.Submission.Id;
                var answerSaved = await context.InsertOrUpdate(answer, token);
                if (!answerSaved) throw new SaveChangesException();
            }

            // When a submission is final, the results of the answers and the grade should be calculated
            if (!request.Submission.IsFinal) return true;
            return await CalculateSubmissionGrade(request.Submission.Id, token);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> CalculateSubmissionGrade(int submissionId, CancellationToken token)
    {
        try
        {
            // Get the submission
            var submissionRequest = new GetSubmissionByIdRequest(submissionId);
            var submission = await Send(submissionRequest, token);
            if (submission == null) return false;
            var lesson = await Send(new GetLessonIncludeReferencesRequest(submission.LessonId), token);

            // Calculate the grades
            var request = new CalculateSubmissionGradeRequest(submissionId);
            submission.Grade = await Send(request, token);
            if (!await IsValidAsync(submission)) return false;

            // Save the submission
            var asyncDataContext = GetAsyncContext();
            var saved = await asyncDataContext.UpdateAsync(submission, token);
            if(saved == false) return false;

            // Get the answer for this lesson
            var context = asyncDataContext.GetDataContext();
            var answers = await context.Answers
                .Where(a => a.SubmissionId == request.SubmissionId)
                .ToListAsync(token);

            // Calculate if the answers are correct
            foreach (var answer in answers)
            {
                var question = lesson.Content?.FirstOrDefault(c => c.Type == answer.QuestionType && c.Id == answer.QuestionId);
                if (question == null) continue;
                answer.IsCorrectAnswer = answer.IsCorrect(question);
            }

            // Save the answers
            context.UpdateRange(answers);
            await context.SaveChangesAsync(token);

            // Done
            return true;
        }
        catch (Exception) { return false; }
    }
}