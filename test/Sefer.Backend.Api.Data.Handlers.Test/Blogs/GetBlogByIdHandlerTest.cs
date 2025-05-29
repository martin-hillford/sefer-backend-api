namespace Sefer.Backend.Api.Data.Handlers.Test.Blogs;

[TestClass]
public class GetBlogByIdHandlerTest : GetEntityByIdHandlerTest<GetBlogByIdRequest, GetBlogByIdHandler, Blog>
{
    [TestInitialize]
    public async Task Init()
    {
        var user = new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987 };
        var context = GetDataContext();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    protected override Task<Blog> GetEntity()
    {
        var context = GetDataContext();
        var user = context.Users.Single();
        return Task.FromResult(new Blog { Name = "test", Permalink = "test", Content = "content", AuthorId = user.Id });
    }
}