namespace Sefer.Backend.Api.Data.Handlers.Test.Testimonies;

[TestClass]
public class DeleteTestimonyHandlerTest : DeleteEntityHandlerTest<DeleteTestimonyRequest, DeleteTestimonyHandler, Testimony>
{
    protected override async Task<List<(Testimony Entity, bool IsValid)>> GetTestData()
    {
        var existing = new Testimony { Content = "test", Name = "test", IsAnonymous = true };
        var context = GetDataContext();
        await context.AddAsync(existing);
        await context.SaveChangesAsync();

        var missing = new Testimony { Content = "test", Name = "test" };
        return
        [
            (existing, true),
            (missing, false)
        ];
    }
}