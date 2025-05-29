namespace Sefer.Backend.Api.Data.Handlers.Test.Entities;

[TestClass]
public abstract class DeleteEntityHandlerTest<TRequest, THandler, TEntity> : HandlerUnitTest
    where TRequest : DeleteEntityRequest<TEntity>
    where THandler : DeleteEntityHandler<TRequest, TEntity>
    where TEntity : class, IEntity
{
    [TestMethod]
    public async Task Handle()
    {
        var data = await GetTestData();
        var provider = GetServiceProvider().AddCaching();
        foreach (var (entity, valid) in data)
        {
            await Handle(provider, entity, valid);
        }
    }

    [TestMethod]
    public async Task Handle_Exception()
    {
        var data = await GetTestData();
        var entity = data.First().Entity;

        var handler = GetHandler(GetServiceProvider(new Exception()));
        var request = GetRequest(entity);
        var deleted = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(deleted);
    }

    [TestMethod]
    public async Task Handle_Null()
    {
        var handler = GetHandler(GetServiceProvider(new Exception()));
        var request = GetRequest(null);
        var deleted = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(deleted);
    }

    private async Task Handle(MockedServiceProvider provider, TEntity entity, bool valid)
    {
        var context = GetDataContext();
        var count = await context.Set<TEntity>().CountAsync();

        var handler = GetHandler(provider);
        var request = GetRequest(entity);

        var deleted = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(valid, deleted);

        var recount = await context.Set<TEntity>().CountAsync();
        Assert.AreEqual(deleted ? count - 1 : count, recount);
    }

    protected abstract Task<List<(TEntity Entity, bool IsValid)>> GetTestData();

    protected virtual THandler GetHandler(MockedServiceProvider provider)
        => TypeExtensions.InvokeFirstConstructor<THandler>(provider.Object);

    protected virtual TRequest GetRequest(TEntity? entity)
        => TypeExtensions.InvokeFirstConstructor<TRequest>(entity);
}