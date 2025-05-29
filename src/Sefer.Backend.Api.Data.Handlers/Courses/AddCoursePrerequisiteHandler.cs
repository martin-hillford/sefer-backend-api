namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class AddCoursePrerequisiteHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddCoursePrerequisiteRequest, CoursePrerequisite>(serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override Task<bool> Handle(AddCoursePrerequisiteRequest request, CancellationToken token)
    {

        var context = GetDataContext();
        var helper = new CoursePrerequisiteHelper(_serviceProvider, context);
        return helper.Add(request.Entity);
    }
}