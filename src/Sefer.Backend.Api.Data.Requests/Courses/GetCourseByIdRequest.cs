namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetCourseByIdRequest : GetEntityByIdRequest<Course>
{
    public readonly bool WithRevision;
    
    public GetCourseByIdRequest(int? id) : base(id) { }
        
    public GetCourseByIdRequest(int? id, bool withRevision) : base(id)
    {
        WithRevision = withRevision;
    }
}