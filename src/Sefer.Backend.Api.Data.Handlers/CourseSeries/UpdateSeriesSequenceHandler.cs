namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class UpdateSeriesSequenceHandler(IServiceProvider serviceProvider)
    : SyncHandler<UpdateSeriesSequenceRequest, bool>(serviceProvider)
{
    protected override bool Handle(UpdateSeriesSequenceRequest request)
    {
        try
        {
            var context = GetDataContext();
            var lookup = context.Series.ToDictionary(c => c.Id);
            if (request.Series == null || request.Series.Count != lookup.Count) return false;

            for (var index = 0; index < request.Series.Count; index++)
            {
                var id = request.Series[index];
                if (lookup.TryGetValue(id, out var series) == false) return false;
                series.SequenceId = index;
            }
            context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}