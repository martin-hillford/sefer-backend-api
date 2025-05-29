namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class AddSubmissionHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddSubmissionRequest, LessonSubmission>(serviceProvider);