namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class GetSeriesByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetSeriesByIdRequest, Series>(serviceProvider);