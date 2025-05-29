namespace Sefer.Backend.Api.Data.Handlers.Test.Blogs;

public abstract class BlogHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var context = GetDataContext();
        await Init(context);
    }

    public static async Task<User> Init(DataContext context)
    {
        var user = new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987 };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var blogs = new List<Blog>
        {
            new () {Name = "test1", Permalink = "test1", Content = "content", AuthorId = user.Id, IsPublished = true, PublicationDate = DateTime.UtcNow},
            new () {Name = "test2", Permalink = "test2", Content = "content", AuthorId = user.Id, IsPublished = true , PublicationDate = DateTime.UtcNow.AddDays(- 1)},
            new () {Name = "test3", Permalink = "test3", Content = "content", AuthorId = user.Id},
        };

        await context.Blogs.AddRangeAsync(blogs);
        await context.SaveChangesAsync();
        return user;
    }
}