namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

[TestClass]
public class IsPublishableCourseRevisionHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_RevisionNull()
    {
        await Handle(GetServiceProvider(), false);
    }
    
    [TestMethod]
    public async Task Handle_InValidRevision()
    {
        var courseRevision = new CourseRevision();
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetCourseRevisionByIdRequest, CourseRevision>(courseRevision);
        await Handle(provider, false);
    }
    
    [TestMethod]
    public async Task Handle_IncorrectStage()
    {
        var courseRevision = new CourseRevision { CourseId = 1, Version = 1, Id = 1, Stage = Stages.Published };
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetCourseRevisionByIdRequest, CourseRevision>(courseRevision);
        await Handle(provider, false);
    }
    
    [TestMethod]
    public async Task Handle_NoLessons()
    {
        var courseRevision = new CourseRevision { CourseId = 1, Version = 1, Id = 1, Stage = Stages.Edit };
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetCourseRevisionByIdRequest, CourseRevision>(courseRevision);
        provider.AddRequestResult<GetLessonsByCourseRevisionRequest, List<Lesson>>([]);
        await Handle(provider, false);
    }
    
    [TestMethod]
    public async Task Handle_NoContent()
    {
        var courseRevision = new CourseRevision { CourseId = 1, Version = 1, Id = 1, Stage = Stages.Edit };
        var lesson = new Lesson { SequenceId = 1};
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetCourseRevisionByIdRequest, CourseRevision>(courseRevision);
        provider.AddRequestResult<GetLessonsByCourseRevisionRequest, List<Lesson>>([lesson]);
        provider.AddRequestResult<GetLessonContentRequest, List<IContentBlock<Lesson>>>([]);
        await Handle(provider, false);
    }
    
    [TestMethod]
    public async Task Handle()
    {
        var courseRevision = new CourseRevision { CourseId = 1, Version = 1, Id = 1, Stage = Stages.Edit };
        var lesson = new Lesson { SequenceId = 1};
        var textElement = new LessonTextElement();
        var content = new List<IContentBlock<Lesson>> {textElement};
        
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetCourseRevisionByIdRequest, CourseRevision>(courseRevision);
        provider.AddRequestResult<GetLessonsByCourseRevisionRequest, List<Lesson>>([lesson]);
        provider.AddRequestResult<GetLessonContentRequest, List<IContentBlock<Lesson>>>(content);
        
        await Handle(provider, true);
    }
    
    private static async Task Handle(MockedServiceProvider provider, bool isPublishable)
    {
        var request = new IsPublishableCourseRevisionRequest(21);
        var handler = new IsPublishableCourseRevisionHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(isPublishable, result);
    }
}