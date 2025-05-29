namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class IsPublishableCourseRevisionHandler(IServiceProvider serviceProvider) : Handler<IsPublishableCourseRevisionRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsPublishableCourseRevisionRequest request, CancellationToken token)
    {
        // First check if the revision itself is eligible
        var revision = await Send(new GetCourseRevisionByIdRequest(request.CourseRevisionId), token);
        if (revision == null) { return false; }

        if (await IsValidAsync(revision) == false) { return false; }
        if (revision.Stage != Stages.Edit && revision.Stage != Stages.Test) { return false; }

        // Check if the course revision has lessons
        var lessons = await Send(new GetLessonsByCourseRevisionRequest(revision.Id), token);
        if (lessons.Count == 0) { return false; }

        // Now for each lesson check if it has content
        foreach (var lesson in lessons)
        {
            var content = await Send(new GetLessonContentRequest(lesson), token);
            if (content.Count == 0) { return false; }
        }

        // Also check the sequence of the lesson
        var sequenceIds = lessons.Select(e => e.SequenceId).Distinct().Count();
        return lessons.Count == sequenceIds;
    }
}