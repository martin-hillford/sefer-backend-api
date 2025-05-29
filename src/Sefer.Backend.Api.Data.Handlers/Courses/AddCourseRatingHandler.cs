namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class AddCourseRatingHandler(IServiceProvider serviceProvider)
    : Handler<AddCourseRatingRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(AddCourseRatingRequest request, CancellationToken token)
    {
        try
        {
            if (request.Rating == null) return false;
            if (request.Rating.Rating > 10 || request.Rating.Id != 0) return false;
            var course = await Send(new GetCourseByIdRequest(request.Rating.CourseId), token);
            if (course == null) return false;

            var context = GetDataContext();
            request.Rating.CreationDate = DateTime.UtcNow;
            context.CourseRatings.Add(request.Rating);
            await context.SaveChangesAsync(token);
        }
        catch (Exception) { return false; }
        return true;
    }
}