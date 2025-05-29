namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class AddCurriculumHandlerTest
    : AddEntityHandlerTest<AddCurriculumRequest, AddCurriculumHandler, Curriculum>
{
    protected override List<(Curriculum Entity, bool Valid)> GetTestData()
    {
        return
        [
            (new Curriculum(), false),
            (new Curriculum { Name = "name" }, false),
            (new Curriculum { Name = "name", Description = "desc", Level = Levels.Advanced }, true)
        ];
    }
}