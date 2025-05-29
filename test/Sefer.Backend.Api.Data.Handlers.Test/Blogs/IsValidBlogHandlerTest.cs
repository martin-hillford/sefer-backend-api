namespace Sefer.Backend.Api.Data.Handlers.Test.Blogs;

[TestClass]
public class IsValidBlogHandlerTest : IsValidEntityHandlerTest<IsBlogValidRequest, IsBlogValidHandler, Blog>
{
    private User? _user;

    [TestInitialize]
    public async Task Init()
    {
        var context = GetDataContext();
        _user = await BlogHandlerTest.Init(context);
    }

    protected override Dictionary<Blog, bool> GetTestData()
    {
        Assert.IsNotNull(_user);
        return new Dictionary<Blog, bool>
        {
            {new Blog {Name = "test1", Permalink = "test", Content = "content", AuthorId = _user.Id}, false},
            {new Blog {Name = "test", Permalink = "test1", Content = "content", AuthorId = _user.Id}, false},
            {new Blog {Name = "test", Permalink = "test", AuthorId = _user.Id}, false},
            {new Blog {Permalink = "test", Content = "content", AuthorId = _user.Id}, false},
            {new Blog {Name = "test", Permalink = "$test@#", Content = "content", AuthorId = _user.Id}, false},
            {new Blog {Name = "test8", Permalink = "test8", Content = "content8", AuthorId = _user.Id}, true},
        };
    }

    protected override IsBlogValidHandler GetHandler(MockedServiceProvider provider)
    {
        var validation = new Mock<ICustomValidationService<Blog>>();

        validation.Setup(v => v.IsValid(It.Is<Blog>(b => b.Name == "test1"))).Returns(Task.FromResult(false));
        validation.Setup(v => v.IsValid(It.Is<Blog>(b => b.Permalink == "test1"))).Returns(Task.FromResult(false));
        validation
            .Setup(v => v.IsValid(It.Is<Blog>(b => b.Permalink != "test1" && b.Name != "test1")))
            .Returns(Task.FromResult(true));

        provider.AddValidationService(validation);

        return new IsBlogValidHandler(provider.Object);
    }
}