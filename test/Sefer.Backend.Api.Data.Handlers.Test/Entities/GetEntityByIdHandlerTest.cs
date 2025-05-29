namespace Sefer.Backend.Api.Data.Handlers.Test.Entities;

public abstract class GetEntityByIdHandlerTest<TRequest, THandler, TEntity> : HandlerUnitTest
    where TRequest : GetEntityByIdRequest<TEntity>
    where THandler : GetEntityByIdHandler<TRequest, TEntity>
    where TEntity : class, IEntity
{
    [TestMethod]
    public async Task Handle()
    {
        var entity = await GetEntity();
        await Add(entity);
        var loaded = await Handle(entity.Id);
        Assert.IsNotNull(loaded);
    }

    [TestMethod]
    public async Task Handle_NotFoundTest()
    {
        var entity = await GetEntity();
        await Add(entity);
        var loaded = await Handle(entity.Id + 1);
        Assert.IsNull(loaded);
    }

    [TestMethod]
    public async Task Handle_EmptyCollectionTest()
    {
        var loaded = await Handle(13);
        Assert.IsNull(loaded);
    }

    [TestMethod]
    public async Task Handle_NullTest()
    {
        var loaded = await Handle(null);
        Assert.IsNull(loaded);
    }

    [TestMethod]
    public async Task Handle_ExceptionTest()
    {
        var provider = GetServiceProvider(new Exception()).AddCaching();
        var handler = GetHandler(provider);
        var request = GetRequest(13);
        var loaded = await handler.Handle(request, CancellationToken.None);
        Assert.IsNull(loaded);
    }

    private async Task Add(TEntity entity)
    {
        var context = GetDataContext();
        await context.Set<TEntity>().AddAsync(entity);
        await context.SaveChangesAsync();
        var count = await context.Set<TEntity>().CountAsync();
        Assert.AreNotEqual(0, count);
    }

    private async Task<TEntity> Handle(int? entityId)
    {
        var provider = GetServiceProvider().AddCaching();
        var handler = GetHandler(provider);
        var request = GetRequest(entityId);
        return await handler.Handle(request, CancellationToken.None);
    }

    protected abstract Task<TEntity> GetEntity();

    protected virtual THandler GetHandler(MockedServiceProvider provider)
        => TypeExtensions.InvokeFirstConstructor<THandler>(provider.Object);

    protected virtual TRequest GetRequest(int? entityId)
        => TypeExtensions.InvokeFirstConstructor<TRequest>(entityId);
}