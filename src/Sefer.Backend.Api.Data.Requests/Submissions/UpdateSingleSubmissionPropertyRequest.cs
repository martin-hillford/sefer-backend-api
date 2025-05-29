namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class UpdateSingleSubmissionPropertyRequest(LessonSubmission entity, string property)
    : UpdateSingleEntityPropertyRequest<LessonSubmission>(entity, property);