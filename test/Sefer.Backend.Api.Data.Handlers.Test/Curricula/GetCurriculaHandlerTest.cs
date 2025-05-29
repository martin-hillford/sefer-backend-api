namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetCurriculaHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var curriculumA = new Curriculum { Name = "Name_A", Description = "Description", Level = Levels.Advanced };
        var curriculumB = new Curriculum { Name = "Name_B", Description = "Description", Level = Levels.Advanced };
        await InsertAsync(curriculumB, curriculumA);

        var revisionA = new CurriculumRevision { CurriculumId = curriculumA.Id, Stage = Stages.Edit };
        var revisionB = new CurriculumRevision { CurriculumId = curriculumB.Id, Stage = Stages.Closed };
        await InsertAsync(revisionA, revisionB);
    }

    [TestMethod]
    public async Task Handle()
    {
        var request = new GetCurriculaRequest(true);
        var handler = new GetCurriculaHandler(GetServiceProvider().Object);
        var curricula = await handler.Handle(request, CancellationToken.None);

        curricula.Count.Should().Be(2);
        curricula.First().Name.Should().Be("Name_A");
        curricula.First().Revisions.Count.Should().Be(1);
    }
}