namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetCourseRatingHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_CourseIdNull()
    {
        var (rating, count) = await Handle(null);
        Assert.AreEqual(0, rating);
        Assert.AreEqual(0, count);
    }
    
    [TestMethod]
    public async Task Handle_NoRatings()
    {
        var (rating, count) = await Handle(19);
        Assert.AreEqual(0, rating);
        Assert.AreEqual(0, count);
    }
    
    [TestMethod]
    public async Task Handle()
    {
        var course = new Course {Name = "course.1", Description = "course.1", Permalink = "course1"};
        await InsertAsync(course);
        await InsertAsync(new CourseRating {CourseId = course.Id, Rating = 8});
        await InsertAsync(new CourseRating {CourseId = course.Id, Rating = 6});
        
        var (rating, count) = await Handle(course.Id);
        Assert.AreEqual(7, rating);
        Assert.AreEqual(2, count);
    }

    private async Task<(byte rating, int count)> Handle(int? courseId)
    {
        var request = new GetCourseRatingRequest(courseId);
        var handler = new GetCourseRatingHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}