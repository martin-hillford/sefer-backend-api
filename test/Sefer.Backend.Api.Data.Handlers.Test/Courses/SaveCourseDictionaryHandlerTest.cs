namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class SaveCourseDictionaryHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        await InsertAsync(new Course { Name = "course.1", Description = "course.1", Permalink = "course1" });
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 1, Stage = Stages.Edit});
    }
    
    [TestMethod]
    public async Task Handle_NoSingleRevision()
    {
        var words = new List<CourseRevisionDictionaryWord>
        {
            new() { CourseRevisionId = 2, Explanation = "Exp", Word = "Word1" },
            new() { CourseRevisionId = 3, Explanation = "Exp", Word = "Word2" },
        };
        
        var result = await Handle(2, words);
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public async Task Handle_NoRevision()
    {
        var words = new List<CourseRevisionDictionaryWord>
        {
            new() { CourseRevisionId = 3, Explanation = "Exp", Word = "Word2" },
        };
        
        var result = await Handle(3, words);
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public async Task Handle_NoValidObject()
    {
        var words = new List<CourseRevisionDictionaryWord>
        {
            new() { CourseRevisionId = 1, Explanation = "Exp", Word = "Word2" },
        };
        
        var result = await Handle(1, words);
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public async Task Handle_InsertOnly()
    {
        var words = new List<CourseRevisionDictionaryWord>
        {
            new() { CourseRevisionId = 1, Explanation = "Exp", Word = "Word2", Language = "en" },
        };
        
        var result = await Handle(1, words);
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public async Task Handle_Update()
    {
        await InsertAsync(new CourseRevisionDictionaryWord { CourseRevisionId = 1, Explanation = "Exp", Word = "Word", Language = "en"});
        var words = new List<CourseRevisionDictionaryWord>
        {
            new() { CourseRevisionId = 1, Explanation = "Exp2", Word = "Word", Language = "sp"},
        };
        
        var result = await Handle(1, words);
        Assert.IsTrue(result);
        
        var context = GetDataContext();
        var updated = context.CourseRevisionDictionaryWords.ToList();
        Assert.AreEqual(1, updated.Count);
        Assert.AreEqual("Exp2", updated.First().Explanation);
        Assert.AreEqual("sp", updated.First().Language);
    }
    
    [TestMethod]
    public async Task Handle_Delete()
    {
        await InsertAsync(new CourseRevisionDictionaryWord { CourseRevisionId = 1, Explanation = "Exp", Word = "Word", Language = "en"});
        var words = new List<CourseRevisionDictionaryWord>
        {
            new() { CourseRevisionId = 1, Explanation = "Exp2", Word = "Word", Language = "sp"},
        };
        
        var result = await Handle(1, words);
        Assert.IsTrue(result);
        
        var context = GetDataContext();
        var updated = context.CourseRevisionDictionaryWords.ToList();
        Assert.AreEqual(1, updated.Count);
        Assert.AreEqual("Exp2", updated.First().Explanation);
        Assert.AreEqual("sp", updated.First().Language);
    }

    private async Task<bool> Handle(int revisionId, List<CourseRevisionDictionaryWord> words)
    {
        var request = new SaveCourseDictionaryRequest(revisionId, words);
        var handler = new SaveCourseDictionaryHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}