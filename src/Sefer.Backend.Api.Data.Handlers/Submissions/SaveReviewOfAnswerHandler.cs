namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class SaveReviewOfAnswerHandler(IServiceProvider serviceProvider)
    : Handler<SaveReviewOfAnswerRequest, bool>(serviceProvider)
{
    public override Task<bool> Handle(SaveReviewOfAnswerRequest request, CancellationToken token)
    {
        var result = Handle(request);
        return Task.FromResult(result);
    }

    private bool Handle(SaveReviewOfAnswerRequest request)
    {
        var context = GetDataContext();
        var answer = context.Answers.SingleOrDefault(a => a.Id == request.AnswerId);
        if (answer == null) return false;

        answer.MentorReview = request.Review;
        context.Answers.Update(answer);
        context.SaveChanges();
        return true;
    }
}