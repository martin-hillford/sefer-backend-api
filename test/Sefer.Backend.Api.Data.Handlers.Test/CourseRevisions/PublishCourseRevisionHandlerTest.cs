namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

[TestClass]
public class PublishCourseRevisionHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        await InsertAsync(new Course { Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true });
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 1 });
        await InsertAsync(new Lesson { Name = "les.1", CourseRevisionId = 1, Number = "1" });
        await InsertAsync(new MultipleChoiceQuestion { LessonId = 1, Content = "test", SequenceId = 4, IsMultiSelect = false });
        await InsertAsync(new MultipleChoiceQuestionChoice { QuestionId = 1, Answer = "test.1", IsCorrectAnswer = true });
        await InsertAsync(new MultipleChoiceQuestionChoice { QuestionId = 1, Answer = "test.2", IsCorrectAnswer = false });
        await InsertAsync(new LessonTextElement { LessonId = 1, Content = "test", Type = ContentBlockTypes.ElementText, SequenceId = 0 });
        await InsertAsync(new OpenQuestion { LessonId = 1, Content = "test", SequenceId = 3 });
        await InsertAsync(new MediaElement { LessonId = 1, Content = "test", SequenceId = 8, Url = "https://test.com" });
        await InsertAsync(new Survey { CourseRevisionId = 1 });
    }

    [TestMethod]
    public async Task Handle_NotPublishable()
    {
        var provider = GetServiceProvider();
        provider.AddRequestResult<IsPublishableCourseRevisionRequest, bool>(false);
        await Handle(provider, 19, false);
    }

    [TestMethod]
    public async Task Handle_NoRevision()
    {
        var provider = GetServiceProvider();
        provider.AddRequestResult<IsPublishableCourseRevisionRequest, bool>(true);
        await Handle(provider, 19, false);
    }

    [TestMethod]
    public async Task Handle_InvalidLesson()
    {
        // Create an invalid lesson
        var context = GetDataContext();
        var lesson = await context.Lessons.SingleAsync();
        lesson.Name = "i";
        context.Update(lesson);
        await context.SaveChangesAsync();

        // Try to publish the revision 
        var provider = GetServiceProvider();
        provider.AddRequestResult<IsPublishableCourseRevisionRequest, bool>(true);
        var courseRevision = await context.CourseRevisions.SingleAsync();
        await Handle(provider, courseRevision.Id, false);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var courseRevision = await context.CourseRevisions.SingleAsync();

        var provider = GetServiceProvider();
        provider.AddRequestResult<IsPublishableCourseRevisionRequest, bool>(true);

        await Handle(provider, courseRevision.Id, true);
        await AssertPublished(courseRevision);
    }

    [TestMethod]
    public async Task Handle_ClosePrevious()
    {
        // Ensure to create a published course revision
        var context = GetDataContext();
        var publishedRevision = await context.CourseRevisions.SingleAsync();
        publishedRevision.Stage = Stages.Published;
        context.Update(publishedRevision);
        await context.SaveChangesAsync();

        // Add a new editable course revision
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 2, Stage = Stages.Edit });
        await InsertAsync(new Lesson { Name = "les.1", CourseRevisionId = 2, Number = "1" });
        await InsertAsync(new LessonTextElement { LessonId = 2, Content = "test", Type = ContentBlockTypes.ElementText, SequenceId = 0 });
        await InsertAsync(new OpenQuestion { LessonId = 2, Content = "test", SequenceId = 3 });
        await InsertAsync(new Survey { CourseRevisionId = 2 });

        // Published the editable revision
        var courseRevision = await context.CourseRevisions.SingleAsync(s => s.Stage == Stages.Edit);
        var provider = GetServiceProvider();
        provider.AddRequestResult<IsPublishableCourseRevisionRequest, bool>(true);

        await Handle(provider, courseRevision.Id, true);
        await AssertPublished(courseRevision);
    }

    private static async Task Handle(MockedServiceProvider provider, int courseRevisionId, bool isPublished)
    {
        var request = new PublishCourseRevisionRequest(courseRevisionId);
        var handler = new PublishCourseRevisionHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(isPublished, result);
    }

    private Task AssertPublished(CourseRevision courseRevision)
    {
        var context = GetDataContext();
        var publishedRevision = context.CourseRevisions.SingleOrDefault(r => r.PredecessorId == courseRevision.Id);
        Assert.IsNotNull(publishedRevision);
        return Task.CompletedTask;
    }
}