namespace Sefer.Backend.Api.Data.Requests.CourseSeries;

public class GetSeriesByIdRequest(int? id) : GetEntityByIdRequest<Series>(id);