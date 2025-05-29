namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

[TestClass]
public class GetQuestionsOfCourseRevisionHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        await InsertAsync(new Course { Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true });
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 1 });
        await InsertAsync(new Lesson { Name = "les.1", CourseRevisionId = 1, Number = "1" });
        await InsertAsync(new LessonTextElement { LessonId = 1, Content = "test", Type = ContentBlockTypes.ElementText, SequenceId = 0 });
        await InsertAsync(new OpenQuestion { LessonId = 1, Content = "test", SequenceId = 3 });
        await InsertAsync(new BoolQuestion { LessonId = 1, Content = "test", SequenceId = 4 });
        await InsertAsync(new BoolQuestion { LessonId = 1, Content = "test", SequenceId = 1 });
        await InsertAsync(new MultipleChoiceQuestion { LessonId = 1, Content = "test", SequenceId = 2 });
        await InsertAsync(new OpenQuestion { LessonId = 1, Content = "test", SequenceId = 6 });
        await InsertAsync(new MultipleChoiceQuestion { LessonId = 1, Content = "test", SequenceId = 5 });
    }

    [TestMethod]
    public async Task Handle_NoCourseRevision()
    {
        var result = await Handle(19);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var provider = GetServiceProvider();
        var courseRevision = await context.CourseRevisions.SingleAsync(s => s.Id == 1);
        provider.AddRequestResult<GetCourseRevisionByIdRequest, CourseRevision>(courseRevision);
        var lessons = await Handle(courseRevision.Id, provider);

        Assert.AreEqual(1, lessons.Count);

        var questions = lessons.First().Questions;
        Assert.AreEqual(6, questions.Count);

        for (var index = 1; index < questions.Count; index++)
        {
            var current = questions[index];
            var previous = questions[index - 1];

            Assert.AreEqual(index + 1, current.SequenceId);
            Assert.AreEqual(previous.SequenceId + 1, current.SequenceId);
        }
    }


    private async Task<List<(Lesson Lesson, List<Question> Questions)>> Handle(int courseRevisionId, MockedServiceProvider? provider = null)
    {
        provider ??= GetServiceProvider();
        var request = new GetQuestionsOfCourseRevisionRequest(courseRevisionId);
        var handler = new GetQuestionsOfCourseRevisionHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}