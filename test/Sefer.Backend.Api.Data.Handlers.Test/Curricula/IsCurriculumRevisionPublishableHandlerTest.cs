namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class IsCurriculumRevisionPublishableHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var context = GetDataContext();

        var course = new Course { Name = "Course", Description = "Course" };
        var curriculum = new Curriculum { Name = "Name", Description = "Description", Level = Levels.Advanced };
        await InsertAsync(context, course); await InsertAsync(context, curriculum);

        var revision = new CurriculumRevision { CurriculumId = curriculum.Id, Stage = Stages.Edit };
        await InsertAsync(context, revision);
    }

    [TestMethod]
    public async Task Handle_RevisionNull()
    {
        await Handle(19);
    }

    [TestMethod]
    [DataRow(Stages.Closed)]
    [DataRow(Stages.Published)]
    public async Task Handle_RevisionClosedOrPublished(Stages stage)
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        revision.Stage = stage;
        context.UpdateSingleProperty(revision, nameof(revision.Stage));

        await Handle(revision.Id);
    }

    [TestMethod]
    public async Task Handle_NoBlocks()
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        await Handle(revision.Id);
    }

    [TestMethod]
    public async Task Handle_NoCourses()
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        var block = new CurriculumBlock { Name = "block", Description = "desc", CurriculumRevisionId = revision.Id };
        await InsertAsync(block);
        await Handle(revision.Id);
    }

    [TestMethod]
    public async Task Handle_CourseNotPublished()
    {
        var context = GetDataContext();
        var revision = await PrepareCourseBlock(context);
        await Handle(revision.Id);
    }

    [TestMethod]
    public async Task Handle_NoYearBlocks()
    {
        var context = GetDataContext();
        var course = context.Courses.Single();
        await InsertAsync(new CourseRevision { CourseId = course.Id, Stage = Stages.Published });

        var revision = await PrepareCourseBlock(context);
        revision.Years = 20;
        context.UpdateSingleProperty(revision, nameof(revision.Years));

        await Handle(revision.Id);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var course = context.Courses.Single();
        await InsertAsync(new CourseRevision { CourseId = course.Id, Stage = Stages.Published });

        var revision = await PrepareCourseBlock(context);
        await Handle(revision.Id, true);
    }

    private async Task<CurriculumRevision> PrepareCourseBlock(DataContext context)
    {
        var revision = context.CurriculumRevisions.Single();
        var course = context.Courses.Single();

        var block = new CurriculumBlock { Name = "block", Description = "desc", CurriculumRevisionId = revision.Id };
        await InsertAsync(block);

        var courseBlock = new CurriculumBlockCourse { BlockId = block.Id, CourseId = course.Id };
        await InsertAsync(courseBlock);
        return revision;
    }

    private async Task Handle(int revisionId, bool expected = false)
    {
        var request = new IsCurriculumRevisionPublishableRequest(revisionId);
        var handler = new IsCurriculumRevisionPublishableHandler(GetServiceProvider().Object);
        var isPublishable = await handler.Handle(request, CancellationToken.None);
        isPublishable.Should().Be(expected);
    }
}