namespace Sefer.Backend.Api.Data.Handlers.Test.Blogs;

[TestClass]
public class AddBlogHandlerTest :
    AddEntityHandlerTest<AddBlogRequest, AddBlogHandler, Blog>
{
    private User? _user;

    [TestInitialize]
    public async Task Init()
    {
        _user = new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987 };
        var context = GetDataContext();
        await context.Users.AddAsync(_user);
        await context.SaveChangesAsync();
    }

    protected override List<(Blog, bool)> GetTestData()
    {
        Assert.IsNotNull(_user);
        return
        [
            (new Blog { Name = "test", Permalink = "test", Content = "content", AuthorId = _user.Id }, true),
            (new Blog { Name = "test", Permalink = "test", AuthorId = _user.Id }, false),
            (new Blog { Permalink = "test", Content = "content", AuthorId = _user.Id }, false),
            (new Blog { Name = "test", Permalink = "$test@#", Content = "content", AuthorId = _user.Id }, false)
        ];
    }
}