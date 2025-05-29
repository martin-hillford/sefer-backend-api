namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class UpdateSubmissionHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateSubmissionRequest, LessonSubmission>(serviceProvider);