namespace Sefer.Backend.Api.Data.Handlers.Entities;

public class UpdateEntityHandler<TRequest, TEntity> : EntityHandler<TRequest, TEntity, bool>
    where TRequest : UpdateEntityRequest<TEntity>
    where TEntity : class, IEntity
{
    protected UpdateEntityHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<bool> Handle(TRequest request, CancellationToken token)
    {
        try
        {
            var valid = await IsValidAsync(request.Entity);
            if (!valid) return false;

            var context = GetAsyncContext();
            return await context.UpdateAsync(request.Entity, token);
        }
        catch (Exception)
        {
            return false;
        }
    }
}