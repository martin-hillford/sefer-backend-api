namespace Sefer.Backend.Api.Data.Handlers.Curricula;

internal class GetCurriculumBlocksHelper : HelperClass
{
    internal GetCurriculumBlocksHelper(DataContext context) : base(context) { }

    internal List<CurriculumBlock> Handle(CurriculumRevision revision)
    {
        return Context.CurriculumRevisions
            .Where(c => c.Id == revision.Id)
            .SelectMany(c => c.Blocks)
            .OrderBy(b => b.Year).ThenBy(b => b.SequenceId)
            .ToList();
    }
}