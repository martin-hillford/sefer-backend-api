namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

public class GetLessonsUnitTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        await InsertAsync(new Course {Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true});
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 1 });
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 1 });
        await InsertAsync(new Lesson { Name = "les.1", CourseRevisionId = 1, Number = "1"});
        await InsertAsync(new Lesson { Name = "les.1", CourseRevisionId = 2, Number = "1"});
        await InsertAsync(new Lesson { Name = "les.2", CourseRevisionId = 1, Number = "2"});
    }
}