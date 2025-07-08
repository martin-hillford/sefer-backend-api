namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetCourseDictionaryHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        await InsertAsync(new Course { Name = "course.1", Description = "course.1", Permalink = "course1" });
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 1 });
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 2 });
        await InsertAsync(new CourseRevisionDictionaryWord { Word = "word1", CourseRevisionId = 1, Explanation = "exp", Language = "en" });
        await InsertAsync(new CourseRevisionDictionaryWord { Word = "word2", CourseRevisionId = 2, Explanation = "exp", Language = "en"});
    }
    
    [TestMethod]
    public async Task Handle()
    {
        var request = new GetCourseDictionaryRequest(2);
        var handler = new GetCourseDictionaryHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("word2", result.First().Word);
    }
}