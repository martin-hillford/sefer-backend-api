namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public abstract class BaseCourseRevisionHandler<TRequest, TResponse>(IServiceProvider serviceProvider)
    : Handler<TRequest, TResponse>(serviceProvider)
    where TRequest : IRequest<TResponse>
{
    protected bool CloseCourseRevision(DataContext context, int courseRevisionId)
    {
        // Please note: this code must run synchronously, else it will cause a lot of problems
        // since context is not thread safe!
        try
        {
            // check if we have a revision that can be closed
            var revision = context.CourseRevisions.SingleOrDefault(r => r.Id == courseRevisionId);

            if (!IsValidEntity(revision)) return false;
            if (revision is not { Stage: Stages.Published }) return false;

            // Everything is created now, only update the current revision
            revision.ModificationDate = DateTime.UtcNow;
            revision.Stage = Stages.Closed;

            context.Update(revision);
            context.SaveChanges();

            return true;
        }
        catch (Exception) { return false; }
    }

    protected List<Lesson> GetLessonsByRevisionId(DataContext context, int revisionId)
    {
        // Please note: this code must run synchronously, else it will cause a lot of problems
        // since context is not thread safe!
        return context.Lessons
            .AsNoTracking()
            .Where(l => l.CourseRevisionId == revisionId)
            .OrderBy(l => l.SequenceId)
            .ToList();
    }

    protected CourseRevision GetPublishedCourseRevision(DataContext context, int courseId)
    {
        return context.CourseRevisions
            .AsNoTracking()
            .Where(r => r.Stage == Stages.Published && r.CourseId == courseId)
            .Include(r => r.Course)
            .FirstOrDefault();
    }

    protected bool Fail(IDbContextTransaction transaction)
    {
        transaction.Rollback();
        return false;
    }
}
