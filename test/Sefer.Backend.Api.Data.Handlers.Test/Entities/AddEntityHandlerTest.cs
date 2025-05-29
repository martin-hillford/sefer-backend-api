namespace Sefer.Backend.Api.Data.Handlers.Test.Entities;

[TestClass]
public abstract class AddEntityHandlerTest<TRequest, THandler, TEntity> : HandlerUnitTest
    where TRequest : AddEntityRequest<TEntity>
    where THandler : AddEntityHandler<TRequest, TEntity>
    where TEntity : class, IEntity
{
    [TestMethod]
    public async Task Handle()
    {
        var data = GetTestData();
        foreach (var (entity, valid) in data)
        {
            await Handle(entity, valid);
        }
    }

    [TestMethod]
    public async Task Handle_Exception()
    {
        var data = GetTestData();
        var entity = data.First().Entity;

        var handler = GetHandler(GetServiceProvider(new Exception()));
        var request = GetRequest(entity);
        var added = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(added);
    }

    [TestMethod]
    public async Task Handle_Null()
    {
        var handler = GetHandler(GetServiceProvider(new Exception()));
        var request = GetRequest(null);
        var added = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(added);
    }

    private async Task Handle(TEntity entity, bool valid)
    {
        var context = GetDataContext();
        var count = await context.Set<TEntity>().CountAsync();

        var handler = GetHandler(GetServiceProvider());
        var request = GetRequest(entity);

        var added = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(valid, added);

        var recount = await context.Set<TEntity>().CountAsync();

        if (!added) Assert.AreEqual(count, recount);
        else Assert.AreEqual(count + 1, recount);

        if (added) Assert.AreNotEqual(0, entity.Id);
    }

    protected abstract List<(TEntity Entity, bool Valid)> GetTestData();

    protected virtual THandler GetHandler(MockedServiceProvider provider)
        => TypeExtensions.InvokeFirstConstructor<THandler>(provider.Object);

    protected virtual TRequest GetRequest(TEntity? entity)
        => TypeExtensions.InvokeFirstConstructor<TRequest>(entity);
}