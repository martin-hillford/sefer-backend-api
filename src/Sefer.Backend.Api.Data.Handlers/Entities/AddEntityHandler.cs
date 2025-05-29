namespace Sefer.Backend.Api.Data.Handlers.Entities;

public abstract class AddEntityHandler<TRequest, TEntity> : EntityHandler<TRequest, TEntity, bool>
    where TRequest : AddEntityRequest<TEntity>
    where TEntity : class, IEntity
{
    protected AddEntityHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<bool> Handle(TRequest request, CancellationToken token)
    {
        try
        {
            var valid = await IsValidAsync(request.Entity);
            if (!valid) return false;

            var context = GetAsyncContext();
            return await context.AddAsync(request.Entity, token);
        }
        catch (Exception)
        {
            return false;
        }
    }
}