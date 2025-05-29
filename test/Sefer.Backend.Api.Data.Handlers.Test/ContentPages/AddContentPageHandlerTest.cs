namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class AddContentPageHandlerTest : AddEntityHandlerTest<AddContentPageRequest, AddContentPageHandler, ContentPage>
{
    protected override List<(ContentPage, bool)> GetTestData()
    {
        return
        [
            (new ContentPage { Content = "test" }, false),
            (new ContentPage { Name = "test1", }, false),
            (new ContentPage { Name = "test1", Content = "" }, false),
            (new ContentPage { Name = "test1", Content = "a" }, true),
            (new ContentPage { Name = "test1", Content = "a", Permalink = "#%$%#^" }, false),
            (new ContentPage { Name = "test1", Content = "a", Permalink = "test1" }, true),
            (new ContentPage { Name = MaxLength, Content = "a" }, false),
            (new ContentPage { Permalink = MaxLength, Content = "a", Name = "test4" }, false)
        ];
    }
}