namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class UpdateSingleSubmissionPropertyHandler(IServiceProvider serviceProvider)
    : UpdateSingleEntityPropertyHandler<UpdateSingleSubmissionPropertyRequest, LessonSubmission>(serviceProvider);