using Sefer.Backend.Api.Models.App;

namespace Sefer.Backend.Api.Views.App;

/// <summary>
/// This is a result from a push.
/// </summary>
public class PushResult
{
    public string Id { get; init; }

    public int LocalId { get; init; }

    public PushResult(LocalEnrollment local)
    {
        Id = local.Id;
        LocalId = local.LocalId;
    }
    
    public PushResult(LessonSubmission submission, LocalSubmission local)
    {
        Id = submission.Id.ToString();
        LocalId = local.LocalId;
    }
}