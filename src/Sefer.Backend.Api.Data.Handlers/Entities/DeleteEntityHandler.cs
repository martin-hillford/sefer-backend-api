namespace Sefer.Backend.Api.Data.Handlers.Entities;

public class DeleteEntityHandler<TRequest, TEntity> : EntityHandler<TRequest, TEntity, bool>
    where TRequest : DeleteEntityRequest<TEntity>
    where TEntity : class, IEntity
{
    protected DeleteEntityHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override Task<bool> Handle(TRequest request, CancellationToken token)
    {
        try
        {
            var context = GetAsyncContext();
            var result = context.Delete<TEntity>(request.EntityId, token);
            return Task.FromResult(result);
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }

    }
}