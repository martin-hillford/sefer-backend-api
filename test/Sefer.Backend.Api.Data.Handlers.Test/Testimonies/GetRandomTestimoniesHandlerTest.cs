namespace Sefer.Backend.Api.Data.Handlers.Test.Testimonies;

[TestClass]
public class GetRandomTestimoniesHandlerTest : TestimonyHandlerTest
{
    [TestMethod]
    [DataRow(2, false, 2), DataRow(20, false, 5) , DataRow(20, true, 2)]
    public async Task Handler(int count, bool homepageOnly, int expected)
    {
        var handler = new GetRandomTestimoniesHandler(GetServiceProvider().Object);
        var request = new GetRandomTestimoniesRequest(count, homepageOnly);
        var testimonies = await handler.Handle(request, CancellationToken.None);
        
        Assert.AreEqual(expected, testimonies.Count);
        if(homepageOnly) Assert.IsTrue(testimonies.All(t => t.CourseId == null));
    }
}