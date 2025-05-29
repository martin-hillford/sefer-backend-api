namespace Sefer.Backend.Api.Data.Handlers.Test.Entities;

public abstract class GetEntitiesHandlerTest<TRequest, THandler, TEntity> : HandlerUnitTest
    where TRequest : GetEntitiesRequest<TEntity>, new()
    where THandler : GetEntitiesHandler<TRequest, TEntity>
    where TEntity : class, IEntity
{
    protected abstract List<TEntity> GetTestData();

    [TestMethod]
    public async Task Handle()
    {
        var data = GetTestData();
        var context = GetDataContext();
        await context.Set<TEntity>().AddRangeAsync(data);
        await context.SaveChangesAsync();
        Assert.AreEqual(data.Count, await GetCount());
    }

    [TestMethod]
    public async Task Handle_Empty()
        => Assert.AreEqual(0, await GetCount());

    private async Task<int> GetCount()
    {
        var request = GetRequest();
        var handler = GetHandler(GetServiceProvider());
        var results = await handler.Handle(request, CancellationToken.None);
        return results.Count;
    }

    protected virtual THandler GetHandler(MockedServiceProvider provider)
        => TypeExtensions.InvokeFirstConstructor<THandler>(provider.Object);

    protected virtual TRequest GetRequest() => new();
}