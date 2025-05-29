namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class GetContentPageByIdHandlerTest : GetEntityByIdHandlerTest<GetContentPageByIdRequest, GetContentPageByIdHandler, ContentPage>
{
    protected override Task<ContentPage> GetEntity()
    {
        var page = new ContentPage
        {   
            Content = "test",
            Name = "test",
            Type = ContentPageType.IndividualPage,
            Permalink = "test",
            IsHtmlContent = false,
            IsPublished = false
        };
        return Task.FromResult(page);
    }
}