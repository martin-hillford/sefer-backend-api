namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class UpdateSingleCoursePropertyHandlerTest :
    UpdateSingleEntityPropertyHandlerTest<UpdateSingleCoursePropertyRequest, UpdateSingleCoursePropertyHandler, Course>
{
    protected override async Task<List<(Course entity, string property, object newValue, bool updated)>> GetTestData()
    {
        var context = GetDataContext();
        var added = new Course { Name = "course.1", Description = "course.1", Permalink = "course1", };
        await context.AddAsync(added);
        await context.SaveChangesAsync();
        Assert.AreEqual(1, await context.Courses.CountAsync());

        return
        [
            (new Course { Name = "course.1", Description = "course.12", Permalink = "course1", }, "Description",
                "course.12", true),
            (new Course { Name = "course.1", Description = "course.12", Permalink = "course12", }, "Permalink",
                "course12", true),
            (new Course { Name = "course.1", Description = "course.12", Permalink = string.Empty, }, "Permalink",
                string.Empty, false)
        ];
    }
}