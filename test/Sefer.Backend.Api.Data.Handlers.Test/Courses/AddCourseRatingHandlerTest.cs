namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class AddCourseRatingHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_RatingNull()
        => await Handle(null, false);

    [TestMethod]
    public async Task Handle_OverRated()
        => await Handle(new CourseRating { Rating = 11 }, false);

    [TestMethod]
    public async Task Handle_NoNullId()
        => await Handle(new CourseRating { Id = 19 }, false);

    [TestMethod]
    public async Task Handle_NoCourse()
        => await Handle(1, 6, false);

    [TestMethod]
    public async Task Handle()
    {
        var course1 = await EnsureCourse();
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetCourseByIdRequest, Course>(course1);
        await Handle(course1.Id, 5, true, provider);
    }

    [TestMethod]
    public async Task Handle_Exception()
    {
        var course1 = await EnsureCourse();
        var provider = GetServiceProvider();
        provider.AddRequestException<GetCourseByIdRequest, Course>();
        await Handle(course1.Id, 5, false, provider);
    }

    private async Task<Course> EnsureCourse()
    {
        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        var context = GetDataContext();
        await context.AddAsync(course1);
        await context.SaveChangesAsync();
        return course1;
    }

    private async Task Handle(int courseId, byte rating, bool added, MockedServiceProvider? provider = null)
    {
        var entity = new CourseRating { CourseId = courseId, Rating = rating, CreationDate = DateTime.Now };
        await Handle(entity, added, provider);
    }

    private async Task Handle(CourseRating? entity, bool added, MockedServiceProvider? provider = null)
    {
        var request = new AddCourseRatingRequest(entity);
        var handler = new AddCourseRatingHandler(provider?.Object ?? GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(added, result);
    }
}