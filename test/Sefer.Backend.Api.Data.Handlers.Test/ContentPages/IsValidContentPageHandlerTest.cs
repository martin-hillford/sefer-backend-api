namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class IsValidContentPageHandlerTest : IsValidEntityHandlerTest<IsContentPageValidRequest, IsContentPageValidHandler, ContentPage>
{
    [TestInitialize]
    public async Task Init()
    {
        var context = GetDataContext();
        await ContentPageHandlerTest.Init(context);
    }

    protected override Dictionary<ContentPage, bool> GetTestData()
    {
        return new Dictionary<ContentPage, bool>
        {
            {new ContentPage {Name = "test1", Permalink = "test", Content = "content" }, false},
            {new ContentPage {Name = "test", Permalink = "test1", Content = "content" }, false},
            {new ContentPage {Name = "test", Permalink = "test" }, false},
            {new ContentPage {Permalink = "test", Content = "content" }, false},
            {new ContentPage {Name = "test", Permalink = "$test@#", Content = "content" }, false},
            {new ContentPage {Name = "test8", Permalink = "test8", Content = "content8" }, true},
        };
    }

    protected override IsContentPageValidHandler GetHandler(MockedServiceProvider provider)
    {
        var validation = new Mock<ICustomValidationService<ContentPage>>();

        validation.Setup(v => v.IsValid(It.Is<ContentPage>(b => b.Name == "test1"))).Returns(Task.FromResult(false));
        validation.Setup(v => v.IsValid(It.Is<ContentPage>(b => b.Permalink == "test1"))).Returns(Task.FromResult(false));
        validation
            .Setup(v => v.IsValid(It.Is<ContentPage>(b => b.Permalink != "test1" && b.Name != "test1")))
            .Returns(Task.FromResult(true));

        provider.AddValidationService(validation);

        return new IsContentPageValidHandler(provider.Object);
    }
}