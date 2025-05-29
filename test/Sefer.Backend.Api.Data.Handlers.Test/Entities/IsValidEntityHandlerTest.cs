namespace Sefer.Backend.Api.Data.Handlers.Test.Entities;

[TestClass]
public abstract class IsValidEntityHandlerTest<TRequest, THandler, TEntity> : HandlerUnitTest
    where TRequest : IsValidEntityRequest<TEntity>
    where THandler : IsValidEntityHandler<TRequest, TEntity>
    where TEntity : class, IEntity
{
    [TestMethod]
    public async Task Handle()
    {
        var data = GetTestData();
        foreach (var (entity, valid) in data)
        {
            var handler = GetHandler(GetServiceProvider());
            var request = GetRequest(entity);
            var isValid = await handler.Handle(request, CancellationToken.None);
            Assert.AreEqual(valid, isValid);
        }
    }
    
    [TestMethod]
    public async Task Handle_Exception()
    {
        var data = GetTestData();
        var entity = data.First().Key;
        
        var handler = GetHandler(GetServiceProvider(new Exception()));
        var request = GetRequest(entity);
        var isValid = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public async Task Handle_Null()
    {
        var handler = GetHandler(GetServiceProvider(new Exception()));
        var request = GetRequest(null);
        var isValid = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(isValid);
    }
    
    protected abstract Dictionary<TEntity, bool> GetTestData();

    protected virtual THandler GetHandler(MockedServiceProvider provider)
        => TypeExtensions.InvokeFirstConstructor<THandler>(provider.Object);
    
    protected virtual TRequest GetRequest(TEntity? entity)
        => TypeExtensions.InvokeFirstConstructor<TRequest>(entity);

}