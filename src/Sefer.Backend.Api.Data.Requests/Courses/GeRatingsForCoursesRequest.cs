namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GeRatingsForCoursesRequest
    : IRequest<Dictionary<int, (byte Rating, int Count)>>  { }

