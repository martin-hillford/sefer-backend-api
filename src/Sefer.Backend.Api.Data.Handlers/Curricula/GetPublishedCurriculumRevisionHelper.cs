namespace Sefer.Backend.Api.Data.Handlers.Curricula;

internal class GetPublishedCurriculumRevisionHelper : HelperClass
{
    internal GetPublishedCurriculumRevisionHelper(DataContext context) : base(context) { }

    public CurriculumRevision Get(int curriculumId)
    {
        var curriculum = Context.Curricula.SingleOrDefault(c => c.Id == curriculumId);
        if (curriculum == null) return null;

        return Context.CurriculumRevisions
            .FirstOrDefault(r => r.Stage == Stages.Published && r.CurriculumId == curriculum.Id);
    }
}