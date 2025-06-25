namespace Sefer.Backend.Api.Data.Handlers.Test.Resources;

[TestClass]
public class GetTemplatesHandlerTest : HandlerUnitTest 
{
    [TestMethod]
    public async Task Test_Handle()
    {
        var expected = PrepareContext();
        var provider = GetServiceProvider();
        var handler = new GetTemplatesHandler(provider.Object);
        var request = new GetTemplatesRequest();

        var templates = await handler.Handle(request, CancellationToken.None);

        templates.Count.Should().Be(1);
        templates.First().Content.Should().Be(expected.Content);
        templates.First().Language.Should().Be(expected.Language);
        templates.First().Name.Should().Be(expected.Name);
        templates.First().Title.Should().Be(expected.Title);
        templates.First().LayoutName.Should().Be(expected.LayoutName);
        templates.First().HasLayout.Should().Be(expected.HasLayout);
    }
    
    private Template PrepareContext()
    {
        var template = new Template
            { Id = 13, Content = "Content", Language = "nl", Name = "template-mail", Title = "title" };
        var context = GetDataContext();
        context.Templates.Add(template);
        context.SaveChanges();
        return template;
    }
}