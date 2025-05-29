namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonChoiceByIdRequest(int? id) : GetEntityByIdRequest<MultipleChoiceQuestionChoice>(id);