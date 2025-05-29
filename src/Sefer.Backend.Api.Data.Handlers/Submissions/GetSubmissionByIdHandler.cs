namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetSubmissionByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetSubmissionByIdRequest, LessonSubmission>(serviceProvider);