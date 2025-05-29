namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetOverallCourseStageHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_Published()
    {
        var stage = await Handle(true, true, true);
        Assert.AreEqual(Stages.Published, stage);
    }
    
    [TestMethod]
    public async Task Handle_Closed()
    {
        var stage = await Handle(false, true, true);
        Assert.AreEqual(Stages.Closed, stage);
    }
    
    [TestMethod]
    public async Task Handle_Edit()
    {
        var stage = await Handle(false, false, true);
        Assert.AreEqual(Stages.Edit, stage);
    }
    
    [TestMethod]
    public async Task Handle_Null()
    {
        var stage = await Handle(false, false, false);
        Assert.AreEqual(null, stage);
    } 
    
    private async Task<Stages?> Handle(bool withPublished, bool withClosed, bool withEditing)
    {
        var provider = GetProvider(withPublished, withClosed, withEditing);
        var request = new GetOverallCourseStageRequest(19);
        var handler = new GetOverallCourseStageHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }

    private MockedServiceProvider GetProvider(bool withPublished, bool withClosed, bool withEditing)
    {
        var mocked = GetServiceProvider();

        var published = withPublished ? new CourseRevision() : null;
        if(published != null) mocked.AddRequestResult<GetPublishedCourseRevisionRequest, CourseRevision>(published);

        var closed = new List<CourseRevision>();
        if(withClosed) closed.Add(new CourseRevision());
        mocked.AddRequestResult<GetClosedCourseRevisionsRequest, List<CourseRevision>>(closed);
        
        var editing = withEditing ? new CourseRevision { Stage = Stages.Edit } : null;
        if(editing != null) mocked.AddRequestResult<GetEditingCourseRevisionRequest, CourseRevision>(editing);

        return mocked;
    }
}
