namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class SaveReviewOfAnswerRequest(int answerId, string review) : IRequest<bool>
{
    public readonly int AnswerId = answerId;

    public readonly string Review = review;
}