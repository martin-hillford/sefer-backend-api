namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetCourseRatingHandler(IServiceProvider serviceProvider)
    : Handler<GetCourseRatingRequest, (byte rating, int count)>(serviceProvider)
{
    public override async Task<(byte rating, int count)> Handle(GetCourseRatingRequest request, CancellationToken token)
    {
        if (request.CourseId == null) return (0, 0);

        await using var context = GetDataContext();
        var ratings = context.CourseRatings.Where(c => c.CourseId == request.CourseId);
        var count = await ratings.CountAsync(token);
        if (count == 0) return (0, 0);

        var sum = await ratings.SumAsync(c => c.Rating, token);
        var rating = (byte)Math.Min(Math.Round(sum / (decimal)count), 255);
        return (rating, count);
    }
}