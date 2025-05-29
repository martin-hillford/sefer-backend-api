namespace Sefer.Backend.Api.Data.Handlers.Test.Entities;

[TestClass]
public abstract class UpdateEntityHandlerTest<TRequest, THandler, TEntity> : HandlerUnitTest
    where TRequest : UpdateEntityRequest<TEntity>
    where THandler : UpdateEntityHandler<TRequest, TEntity>
    where TEntity : class, IEntity
{
    [TestMethod]
    public async Task Handle()
    {
        var data = await GetTestData();
        foreach (var (entity, valid) in data)
        {
            await Handle(entity, valid);
        }
    }

    [TestMethod]
    public async Task Handle_Exception()
    {
        var data = await GetTestData();
        var entity = data.First().entity;

        var handler = GetHandler(GetServiceProvider(new Exception()));
        var request = GetRequest(entity);
        var updated = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(updated);
    }

    [TestMethod]
    public async Task Handle_Null()
    {
        var handler = GetHandler(GetServiceProvider(new Exception()));
        var request = GetRequest(null);
        var updated = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(updated);
    }

    private async Task Handle(TEntity entity, bool valid)
    {
        await using var context = GetDataContext();
        var count = await context.Set<TEntity>().CountAsync();

        var provider = GetServiceProvider().AddCaching();
        var handler = GetHandler(provider);
        var request = GetRequest(entity);

        var updated = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(valid, updated);

        var recount = await context.Set<TEntity>().CountAsync();
        Assert.AreEqual(count, recount);
    }

    protected abstract Task<List<(TEntity entity, bool valid)>> GetTestData();

    protected virtual THandler GetHandler(MockedServiceProvider provider)
        => TypeExtensions.InvokeFirstConstructor<THandler>(provider.Object);

    protected virtual TRequest GetRequest(TEntity? entity)
        => TypeExtensions.InvokeFirstConstructor<TRequest>(entity);

}