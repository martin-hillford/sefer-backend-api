namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

public abstract class CurriculumUnitTest : HandlerUnitTest
{
    protected async Task InitializeCurriculumBlock()
    {
        var context = GetDataContext();
        await InitializeCurriculumBlock(context);
    }

    protected async Task InitializeCurriculumBlockCourse()
    {
        var context = GetDataContext();
        await InitializeCurriculumBlockCourse(context);
    }

    public static async Task InitializeCurriculumBlockCourse(DataContext context)
    {
        await InitializeCurriculumBlock(context);

        var block = await context.CurriculumBlocks.FirstAsync();
        var course = await context.Courses.FirstAsync();
        var courseBlock = new CurriculumBlockCourse { BlockId = block.Id, CourseId = course.Id, SequenceId = 1 };
        await InsertAsync(context, courseBlock);
    }

    public static async Task InitializeCurriculumBlock(DataContext context)
    {
        var course = new Course { Name = "Course", Description = "Course" };
        var curriculum = new Curriculum { Name = "Name", Description = "Description", Level = Levels.Advanced, Permalink = "name" };
        await InsertAsync(context, course); await InsertAsync(context, curriculum);

        var revision = new CurriculumRevision { CurriculumId = curriculum.Id, Stage = Stages.Edit };
        var courseRevision = new CourseRevision { CourseId = course.Id, Stage = Stages.Edit };
        await InsertAsync(context, revision);
        await InsertAsync(context, courseRevision);

        var block = new CurriculumBlock { Name = "block", Description = "desc", CurriculumRevisionId = revision.Id };
        await InsertAsync(context, block);
    }
}