namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class UpdateContentPageHandlerTest : UpdateEntityHandlerTest<UpdateContentPageRequest, UpdateContentPageHandler, ContentPage>
{
    protected override async Task<List<(ContentPage, bool)>> GetTestData()
    {
        var existing = new ContentPage { Content = "test", Name = "test" };
        var context = GetDataContext();
        await context.AddAsync(existing);
        await context.SaveChangesAsync();

        var missing = new ContentPage { Content = "test", Name = "test" };
        var invalid = new ContentPage { Id = existing.Id };
        return
        [
            (existing, true),
            (missing, false),
            (invalid, false)
        ];
    }
}