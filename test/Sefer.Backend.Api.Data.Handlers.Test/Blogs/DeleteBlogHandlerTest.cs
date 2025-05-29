namespace Sefer.Backend.Api.Data.Handlers.Test.Blogs;

[TestClass]
public class DeleteBlogHandlerTest : DeleteEntityHandlerTest<DeleteBlogRequest, DeleteBlogHandler, Blog>
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
        list.Add((new Blog { Id = 231 }, false));

        return list;
    }
}