namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetCoursesHandlerTest :  GetEntitiesHandlerTest<GetCoursesRequest, GetCoursesHandler, Course>
{
    protected override List<Course> GetTestData() =>
    [
        new Course { Name = "course.1", Description = "course.1", Permalink = "course1" },
        new Course { Name = "course.2", Description = "course.2", Permalink = "course2" },
        new Course { Name = "course.3", Description = "course.3", Permalink = "course3" }
    ];
}