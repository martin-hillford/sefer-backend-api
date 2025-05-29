namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetPublishedCoursesBySeriesHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        var course2 = new Course { Name = "course.2", Description = "course.2", Permalink = "course3" };
        var course3 = new Course { Name = "course.3", Description = "course.3", Permalink = "course4" };

        await InsertAsync(course1, course2, course3);
        await InsertAsync(new CourseRevision { CourseId = course1.Id, Stage = Stages.Edit, Version = 1 });
        await InsertAsync(new CourseRevision { CourseId = course2.Id, Stage = Stages.Published, Version = 1 });
        await InsertAsync(new CourseRevision { CourseId = course3.Id, Stage = Stages.Published, Version = 1 });

        var series = new Series { Name = "Series", Description = "Series", Level = Levels.Intermediate };

        await InsertAsync(series);

        await InsertAsync(new SeriesCourse { CourseId = course1.Id, SeriesId = series.Id, SequenceId = 1 });
        await InsertAsync(new SeriesCourse { CourseId = course2.Id, SeriesId = series.Id, SequenceId = 2 });
        await InsertAsync(new SeriesCourse { CourseId = course3.Id, SeriesId = series.Id, SequenceId = 3 });
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var series = await context.Series.FirstAsync();
        var result = await Handle(series.Id);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("course.2", result.First().Name);
        Assert.AreEqual("course.3", result.Last().Name);
    }

    [TestMethod]
    public async Task Handle_NoSeries()
    {
        var result = await Handle(19);
        Assert.AreEqual(0, result.Count);
    }

    private async Task<List<Course>> Handle(int seriesId)
    {
        var request = new GetPublishedCoursesBySeriesRequest(seriesId);
        var handler = new GetPublishedCoursesBySeriesHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}