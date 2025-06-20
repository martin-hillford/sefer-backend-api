namespace Sefer.Backend.Api.Data.Requests.Courses;

public class DeleteCoursePrerequisiteRequest(CoursePrerequisite entity) : IRequest<bool>
{
    public int CourseId => entity.CourseId;
    
    public int RequiredCourseId => entity.RequiredCourseId;
}