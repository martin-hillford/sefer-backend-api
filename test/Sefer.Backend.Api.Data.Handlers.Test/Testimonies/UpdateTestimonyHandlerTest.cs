namespace Sefer.Backend.Api.Data.Handlers.Test.Testimonies;

[TestClass]
public class UpdateTestimonyHandlerTest : UpdateEntityHandlerTest<UpdateTestimonyRequest, UpdateTestimonyHandler, Testimony>
{
    protected override async Task<List<(Testimony, bool)>> GetTestData()
    {
        var existing = new Testimony { Content = "test-5", Name = "test" };
        var context = GetDataContext();
        await context.AddAsync(existing);
        await context.SaveChangesAsync();

        var missing = new Testimony { Content = "test-5", Name = "test" };
        var invalid = new Testimony { Id = existing.Id };
        return
        [
            (existing, true),
            (missing, false),
            (invalid, false)
        ];
    }
}