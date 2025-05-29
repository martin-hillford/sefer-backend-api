namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

[TestClass]
public class ContentStateHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        await InsertAsync(new Course {Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true});
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 1 });
    }
    
    [TestMethod]
    public async Task Handle_CourseRevisionNull()
    {
        var state = await Handle(19);
        Assert.IsTrue(state == null || !state.Any());
    }
    
    [TestMethod]
    public async Task Handle_EmptyCourseRevision()
    {
        var state = await Handle(1);
        Assert.IsTrue(state == null || !state.Any());
    }
    
    [TestMethod]
    public async Task Handle_EmptyLesson()
    {
        await InsertAsync(new Lesson { Name = "les.1", CourseRevisionId = 1, Number = "1"});
        var actual = await Handle(1);
        Assert.IsTrue(actual.All(l => l.ContentState == ContentState.Html));
    }
    
    [TestMethod]
    [DataRow(ContentState.Html, false)]
    [DataRow(ContentState.MarkDown, true)]
    public async Task Handle_MultipleSame(ContentState expectedState, bool isMarkDown)
    {
        await InsertAsync(new Lesson { Name = "les.1", CourseRevisionId = 1, Number = "1"});
        await InsertAsync(new LessonTextElement { LessonId = 1, Content = "test", IsMarkDownContent = isMarkDown, Type = ContentBlockTypes.ElementText });
        await InsertAsync(new BoolQuestion { LessonId = 1, Content = "test", IsMarkDownContent = isMarkDown });
        await InsertAsync(new MultipleChoiceQuestion { LessonId = 1, Content = "test", IsMarkDownContent = isMarkDown });
        await InsertAsync(new OpenQuestion { LessonId = 1, Content = "test", IsMarkDownContent = isMarkDown });
        await InsertAsync(new MediaElement { LessonId = 1, Content = "test", Url = "test", IsMarkDownContent = isMarkDown });
        var actual = await Handle(1);
        Assert.IsTrue(actual.All(l => l.ContentState == expectedState));
    }

    [TestMethod]
    public async Task Handle_Mixed()
    {
        await InsertAsync(new Lesson { Name = "les.1", CourseRevisionId = 1, Number = "1"});
        await InsertAsync(new LessonTextElement { LessonId = 1, Content = "test", IsMarkDownContent = true, Type = ContentBlockTypes.ElementText });
        await InsertAsync(new BoolQuestion { LessonId = 1, Content = "test", IsMarkDownContent = false });
        await InsertAsync(new MultipleChoiceQuestion { LessonId = 1, Content = "test", IsMarkDownContent = true });
        await InsertAsync(new OpenQuestion { LessonId = 1, Content = "test", IsMarkDownContent = false });
        await InsertAsync(new MediaElement { LessonId = 1, Content = "test", Url = "test", IsMarkDownContent = true });
        var actual = await Handle(1);
        Assert.IsTrue(actual.All(l => l.ContentState == ContentState.Mixed));
    }
    
    private async Task<List<LessonContentState>> Handle(int courseRevisionId)
    {
        var request = new ContentStateRequest(courseRevisionId);
        var handler = new ContentStateHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}