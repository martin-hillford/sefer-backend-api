namespace Sefer.Backend.Api.Data.Handlers.Test.Testimonies;

[TestClass]
public class GetTestimoniesHandlerTest : GetEntitiesHandlerTest<GetTestimoniesRequest, GetTestimoniesHandler, Testimony>
{
    protected override List<Testimony> GetTestData() =>
    [
        new Testimony { Content = "test1", Name = "test1", IsAnonymous = true, },
        new Testimony { Content = "test2", Name = "test2", IsAnonymous = true, }
    ];
}