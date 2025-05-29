namespace Sefer.Backend.Api.Data.Handlers.Test.Testimonies;

[TestClass]
public class GetTestimonyByIdHandlerTest
    : GetEntityByIdHandlerTest<GetTestimonyByIdRequest, GetTestimonyByIdHandler, Testimony>
{
    protected override Task<Testimony> GetEntity()
    {
        var testimony = new Testimony { Content = "overall.1", Name = "overall.1", IsAnonymous = true, CreationDate = DateTime.Now };
        return Task.FromResult(testimony);
    }
}