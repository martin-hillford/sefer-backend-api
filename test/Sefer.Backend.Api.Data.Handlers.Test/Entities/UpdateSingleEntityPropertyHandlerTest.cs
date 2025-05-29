namespace Sefer.Backend.Api.Data.Handlers.Test.Entities;

public abstract class UpdateSingleEntityPropertyHandlerTest<TRequest, THandler, TEntity> : HandlerUnitTest
    where TRequest : UpdateSingleEntityPropertyRequest<TEntity>
    where THandler : UpdateSingleEntityPropertyHandler<TRequest, TEntity>
    where TEntity : class, IEntity
{
    protected abstract Task<List<(TEntity entity, string property, object newValue, bool updated)>> GetTestData();

    [TestMethod]
    public async Task Handle()
    {
        var data = await GetTestData();
        foreach (var (entity, property, newValue, updated) in data)
        {
            var handler = GetHandler(GetServiceProvider().AddCaching());
            var request = GetRequest(entity, property);
            var result = await handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(updated, result);
            if (updated) Assert.IsTrue(await HasValue(entity, property, newValue));
        }
    }

    [TestMethod]
    public async Task Handle_Null()
    {
        var handler = GetHandler(GetServiceProvider());
        var request = GetRequest(null, "property");
        var result = await handler.Handle(request, CancellationToken.None);

        Assert.IsFalse(result);
    }


    [TestMethod]
    public async Task Handle_Exception()
    {
        var handler = GetHandler(GetServiceProvider(new Exception()));
        var request = GetRequest(null, "property");
        var result = await handler.Handle(request, CancellationToken.None);

        Assert.IsFalse(result);
    }

    protected virtual THandler GetHandler(MockedServiceProvider provider)
        => TypeExtensions.InvokeFirstConstructor<THandler>(provider.Object);

    protected virtual TRequest GetRequest(TEntity? entity, string property)
        => TypeExtensions.InvokeFirstConstructor<TRequest>([entity, property]);

    private async Task<bool> HasValue(TEntity entity, string property, object? value)
    {
        var context = GetDataContext();
        var instance = await context.Set<TEntity>().SingleOrDefaultAsync(e => e.Id == entity.Id);
        if (instance == null) return false;

        var instanceValue = typeof(TEntity).GetProperty(property)?.GetValue(instance);
        if (instanceValue == null) return value == null;
        return instanceValue.Equals(value);
    }
}