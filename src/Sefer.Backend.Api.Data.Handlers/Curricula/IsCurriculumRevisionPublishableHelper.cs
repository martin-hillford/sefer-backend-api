namespace Sefer.Backend.Api.Data.Handlers.Curricula;

internal class IsCurriculumRevisionPublishableHelper : HelperClass
{
    internal IsCurriculumRevisionPublishableHelper(IServiceProvider serviceProvider, DataContext context)
        : base(serviceProvider, context) { }

    internal bool IsPublishable(CurriculumRevision revision)
    {
        // First check if the revision itself is eligible
        if (revision == null) return false;
        if (!BaseValidation.IsValidEntity(ServiceProvider, revision)) return false;
        if (revision.Stage != Stages.Edit && revision.Stage != Stages.Test) return false;

        // Check if the curriculum revision has block
        var blocksHelper = new GetCurriculumBlocksHelper(Context);
        var blocks = blocksHelper.Handle(revision);
        if (blocks.Any() == false) return false;

        // Now for each block check if it has courses
        var coursesHelper = new GetCoursesByCurriculumBlockHelper(Context);
        foreach (var block in blocks)
        {
            var published = coursesHelper.Handle(block.Id, true);
            var courses = coursesHelper.Handle(block.Id, false);
            if (courses.Count == 0 || courses.Count != published.Count) return false;
        }

        // Also checks if there are blocks for all years
        var years = blocks.Select(b => b.Year).Distinct().Count();
        return revision.Years <= 0 || years >= revision.Years;
    }
}