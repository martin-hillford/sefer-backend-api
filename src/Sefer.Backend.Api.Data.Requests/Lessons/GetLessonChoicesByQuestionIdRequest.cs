namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonChoicesByQuestionIdRequest(int questionId) : IRequest<List<MultipleChoiceQuestionChoice>>
{
    public readonly int QuestionId = questionId;
}