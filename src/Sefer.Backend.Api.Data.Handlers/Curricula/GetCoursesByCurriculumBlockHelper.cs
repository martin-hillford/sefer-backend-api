namespace Sefer.Backend.Api.Data.Handlers.Curricula;

internal class GetCoursesByCurriculumBlockHelper : HelperClass
{
    internal GetCoursesByCurriculumBlockHelper(DataContext context) : base(context) { }

    internal List<Course> Handle(int curriculumBlockId, bool publishedOnly)
    {
        var block = Context.CurriculumBlocks.SingleOrDefault(b => b.Id == curriculumBlockId);
        if (block == null) { return new List<Course>(); }

        var query = Context.CurriculumBlockCourses
            .Where(b => b.BlockId == curriculumBlockId)
            .Where(b => !publishedOnly || b.Course.CourseRevisions.Any(r => r.Stage == Stages.Published));

        if (publishedOnly) return query.OrderBy(b => b.Course.Name).Select(b => b.Course).ToList();
        return query.OrderBy(b => b.SequenceId).Select(b => b.Course).ToList();
    }
}