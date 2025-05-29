namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class DeleteLessonChoiceRequest(IEnumerable<MultipleChoiceQuestionChoice> choices) : IRequest<bool>
{
    public readonly IEnumerable<MultipleChoiceQuestionChoice> Choices = choices;

    public DeleteLessonChoiceRequest(MultipleChoiceQuestionChoice choice)
        : this(new List<MultipleChoiceQuestionChoice> {choice}) { }
}