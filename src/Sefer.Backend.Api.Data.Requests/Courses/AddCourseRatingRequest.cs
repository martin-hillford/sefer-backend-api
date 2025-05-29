namespace Sefer.Backend.Api.Data.Requests.Courses;

public class AddCourseRatingRequest(CourseRating rating) : IRequest<bool>
{
    public readonly CourseRating Rating = rating;
}