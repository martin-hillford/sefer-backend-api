namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GeRatingsForCoursesHandler(IServiceProvider serviceProvider)
    : Handler<GeRatingsForCoursesRequest, Dictionary<int, (byte Rating, int Count)>>(serviceProvider)
{
    public override async Task<Dictionary<int, (byte Rating, int Count)>> Handle(GeRatingsForCoursesRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();

        // Please note: SQL version:
        // SELECT CourseId, CAST(AVG(CAST(Rating as FLOAT)) as tinyint) AS Rating, COUNT(*) as Count  FROM CourseRatings GROUP BY CourseId
        return await context.CourseRatings
            .GroupBy(c => c.CourseId)
            .Select(g => new { CourseId = g.Key, Count = g.Count(), Rating = (byte)g.Average(c => (float)c.Rating) })
            .ToDictionaryAsync(g => g.CourseId, g => (g.Rating, g.Count), token);
    }
}

