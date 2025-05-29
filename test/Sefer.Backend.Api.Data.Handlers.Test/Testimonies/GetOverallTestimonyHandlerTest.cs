namespace Sefer.Backend.Api.Data.Handlers.Test.Testimonies;

[TestClass]
public class GetOverallTestimonyHandlerTest : TestimonyHandlerTest
{
    [TestMethod]
    [DataRow(null), DataRow(0), DataRow(2), DataRow(3)]
    public async Task Handle(int? limit)
    {
        Assert.IsTrue(limit is not 1);
        var testimonies = await Get(limit);
        Assert.AreEqual(2, testimonies.Count);
        
        Assert.AreEqual("overall.2", testimonies.First().Name);
        Assert.AreEqual("overall.1", testimonies.Last().Name);
    }
    
    [TestMethod]
    public async Task Handle_Take()
    {
        var testimonies = await Get(1);
        Assert.AreEqual(1, testimonies.Count);
        Assert.AreEqual("overall.2", testimonies.First().Name);
    }
    
    private async Task<List<Testimony>> Get(int? limit)
    {
        var handler = new GetOverallTestimoniesHandler(GetServiceProvider().Object);
        var request = new GetOverallTestimoniesRequest(limit);
        return await handler.Handle(request, CancellationToken.None);
    }
}