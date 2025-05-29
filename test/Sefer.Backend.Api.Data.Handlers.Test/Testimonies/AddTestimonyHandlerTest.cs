namespace Sefer.Backend.Api.Data.Handlers.Test.Testimonies;

[TestClass]
public class AddTestimonyHandlerTest
    : AddEntityHandlerTest<AddTestimonyRequest, AddTestimonyHandler, Testimony>
{
    protected override List<(Testimony, bool)> GetTestData() =>
    [
        (new Testimony { Content = "", Name = "" }, false),
        (new Testimony { Content = "test-5", Name = "Test" }, true)
    ];
}