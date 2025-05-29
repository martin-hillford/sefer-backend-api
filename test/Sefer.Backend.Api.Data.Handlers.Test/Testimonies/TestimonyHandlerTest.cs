namespace Sefer.Backend.Api.Data.Handlers.Test.Testimonies;

public abstract class TestimonyHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        await using var c = GetDataContext();

        var user1 = new User { Name = "user.1", Email = "user.1@example.com" };
        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course.1" };
        var course2 = new Course { Name = "course.2", Description = "course.2", Permalink = "course.2" };
        await c.AddRangeAsync(new List<Course> { course1, course2 });
        await c.AddAsync(user1);
        await c.SaveChangesAsync();

        await c.AddAsync(new Testimony { Content = "overall.1", Name = "overall.1", IsAnonymous = true, CreationDate = DateTime.Now });
        await c.AddAsync(new Testimony { Content = "overall.2", Name = "overall.2", StudentId = user1.Id, CreationDate = DateTime.Now.AddDays(1) });
        await c.AddAsync(new Testimony { Content = "course.1", Name = "course.1", CourseId = course1.Id, CreationDate = DateTime.Now.AddDays(2) });
        await c.AddAsync(new Testimony { Content = "course.2", Name = "course.2", CourseId = course2.Id, CreationDate = DateTime.Now.AddDays(3) });
        await c.AddAsync(new Testimony { Content = "course.2", Name = "course.2", CourseId = course2.Id, StudentId = user1.Id, CreationDate = DateTime.Now.AddDays(4) });

        await c.SaveChangesAsync();
    }
}