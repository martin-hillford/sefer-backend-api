namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class DeleteSeriesHandler(IServiceProvider serviceProvider)
    : SyncHandler<DeleteSeriesRequest, bool>(serviceProvider)
{
    protected override bool Handle(DeleteSeriesRequest request)
    {
        var context = GetDataContext();
        var transaction = context.BeginTransaction();
        try
        {
            if (request.Series == null) return false;
            var seriesCourses = context.SeriesCourses.Where(sc => sc.SeriesId == request.Series.Id);
            context.SeriesCourses.RemoveRange(seriesCourses);
            context.SaveChanges();
            context.Remove(request.Series);
            context.SaveChanges();
            transaction.Commit();
            return true;
        }
        catch (Exception)
        {
            transaction.Rollback();
            return false;
        }
    }
}