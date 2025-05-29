namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class PublishCurriculumRevisionHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var context = GetDataContext();
        await Initialize(context);
    }

    public static async Task Initialize(DataContext context)
    {
        var course = new Course { Name = "Course", Description = "Course" };
        var curriculum = new Curriculum { Name = "Name", Description = "Description", Level = Levels.Advanced };
        await InsertAsync(context, course); await InsertAsync(context, curriculum);
        await InsertAsync(context, new CourseRevision { CourseId = course.Id, Stage = Stages.Published });

        var revision = new CurriculumRevision { CurriculumId = curriculum.Id, Stage = Stages.Edit };
        await InsertAsync(context, revision);

        var block = new CurriculumBlock { Name = "block", Description = "desc", CurriculumRevisionId = revision.Id };
        await InsertAsync(context, block);

        var courseBlock = new CurriculumBlockCourse { BlockId = block.Id, CourseId = course.Id };
        await InsertAsync(context, courseBlock);
    }

    [TestMethod]
    public async Task Handle_RevisionNull()
    {
        await Handle(19, false);
    }

    [TestMethod]
    public async Task Handle_NotPublishable()
    {
        var context = GetDataContext();
        var courseRevision = context.CourseRevisions.Single();
        context.Remove(courseRevision);
        await context.SaveChangesAsync();

        var revision = context.CurriculumRevisions.Single();
        await Handle(revision.Id, false);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        await Handle(revision.Id, true);
    }

    [TestMethod]
    public async Task Handle_WithPreviousPublishedRevision()
    {
        // Publish the curriculum once
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        await Handle(revision.Id, true);

        // And then publish the created revision
        await using var dataContext = GetContext();
        var editRevision = dataContext.CurriculumRevisions.Single(r => r.Stage == Stages.Edit);

        editRevision.Id.Should().NotBe(revision.Id);
        await Handle(editRevision.Id, true, 3);
    }

    [TestMethod]
    public async Task Handle_Rewards()
    {
        var context = GetDataContext();
        var reward = new Reward { Type = RewardTypes.Curriculum, Description = "test", Name = "test" };
        await InsertAsync(reward);

        var revision = context.CurriculumRevisions.Single();
        await Handle(revision.Id, true);

        await using var dataContext = GetContext();
        dataContext.RewardTargets.Count().Should().Be(1);

    }

    private async Task Handle(int revisionId, bool expectPublished, int expectedCount = 2)
    {
        var request = new PublishCurriculumRevisionRequest(revisionId);
        var provider = GetServiceProvider().Object;
        var handler = new PublishCurriculumRevisionHandler(provider);
        var published = await handler.Handle(request, CancellationToken.None);
        published.Should().Be(expectPublished);

        if (!expectPublished) return;

        var context = GetDataContext();
        context.CurriculumRevisions.Count().Should().Be(expectedCount);
        context.CurriculumBlocks.Count().Should().Be(expectedCount);
        context.CurriculumBlockCourses.Count().Should().Be(expectedCount);
    }
}