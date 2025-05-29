namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetSubmissionByIdRequest(int? id) : GetEntityByIdRequest<LessonSubmission>(id);