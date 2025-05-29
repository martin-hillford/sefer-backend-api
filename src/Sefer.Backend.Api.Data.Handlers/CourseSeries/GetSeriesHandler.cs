namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class GetSeriesHandler(IServiceProvider serviceProvider)
    : GetEntitiesHandler<GetSeriesRequest, Series>(serviceProvider);