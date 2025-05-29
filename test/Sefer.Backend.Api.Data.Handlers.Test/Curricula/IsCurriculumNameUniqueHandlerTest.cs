namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class IsCurriculumNameUniqueHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlock();

    [TestMethod]
    public async Task Handle_NameEmpty()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        await Handle(curriculum.Id, null, false);
        await Handle(curriculum.Id, string.Empty, false);
        await Handle(curriculum.Id, "     ", false);
    }

    [TestMethod]
    public async Task Handle_DifferentCurriculum()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        await Handle(curriculum.Id + 1, curriculum.Name, false);
    }

    [TestMethod]
    public async Task Handle_SameCurriculum()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        await Handle(curriculum.Id, curriculum.Name, true);
    }

    [TestMethod]
    [DataRow("name", false)]
    [DataRow("name ", false)]
    [DataRow("Name", false)]
    [DataRow("Name 2", true)]
    public async Task Handle(string requestName, bool expected)
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        await Handle(curriculum.Id + 1, requestName, expected);
    }

    private async Task Handle(int? curriculumId, string? requestName, bool expected)
    {
        var request = new IsCurriculumNameUniqueRequest(curriculumId, requestName);
        var handler = new IsCurriculumNameUniqueHandler(GetServiceProvider().Object);
        var isUnique = await handler.Handle(request, CancellationToken.None);
        isUnique.Should().Be(expected);
    }
}