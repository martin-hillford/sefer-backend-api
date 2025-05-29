namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

public abstract class ContentPageHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public virtual async Task Init()
    {
        var context = GetDataContext();
        await Init(context);
    }

    public static async Task Init(DataContext context)
    {
        var pages = new List<ContentPage>
        {
            new () {Name = "test1", Permalink = "test1", Content = "content", IsPublished = true, Type = ContentPageType.MenuPage},
            new () {Name = "test2", Permalink = "test2", Content = "content", IsPublished = true , Type = ContentPageType.IndividualPage},
            new () {Name = "test3", Permalink = "test3", Content = "content", },
        };
        await context.ContentPages.AddRangeAsync(pages);
        await context.SaveChangesAsync();
    }
}