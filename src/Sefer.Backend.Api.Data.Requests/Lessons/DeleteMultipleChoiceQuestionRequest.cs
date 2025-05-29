namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class DeleteMultipleChoiceQuestionRequest : DeleteEntityRequest<MultipleChoiceQuestion>
{
    public DeleteMultipleChoiceQuestionRequest(MultipleChoiceQuestion entity) : base(entity) { }
}