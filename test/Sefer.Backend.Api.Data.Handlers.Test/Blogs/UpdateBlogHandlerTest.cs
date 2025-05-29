namespace Sefer.Backend.Api.Data.Handlers.Test.Blogs;

[TestClass]
public class UpdateBlogHandlerTest : UpdateEntityHandlerTest<UpdateBlogRequest, UpdateBlogHandler, Blog>
{
    private User? _user;

    [TestInitialize]
    public async Task Init()
    {
        var context = GetDataContext();
        _user = await BlogHandlerTest.Init(context);
    }

    protected override async Task<List<(Blog, bool)>> GetTestData()
    {
        Assert.IsNotNull(_user);

        var context = GetDataContext();
        var blogs = await context.Blogs.ToListAsync();

        var list = blogs.Select(b => (b, true)).ToList();
        list.Add((new Blog { Name = "test4", Content = "test4", Permalink = "test4", AuthorId = 19 }, false));
        list.Add((new Blog { Name = "test1", Content = "test1", Permalink = "test1", Id = 1 }, false));

        return list;
    }
}