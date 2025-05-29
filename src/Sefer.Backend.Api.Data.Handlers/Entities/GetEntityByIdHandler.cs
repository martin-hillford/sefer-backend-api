namespace Sefer.Backend.Api.Data.Handlers.Entities;

public class GetEntityByIdHandler<TRequest, TEntity> : EntityHandler<TRequest, TEntity, TEntity>
    where TRequest : GetEntityByIdRequest<TEntity>
    where TEntity : class, IEntity
{
    protected GetEntityByIdHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<TEntity> Handle(TRequest request, CancellationToken token)
    {
        try
        {
            if (request.Id == null) return null;
            await using var context = GetDataContext();
            var entitySet = context.Set<TEntity>().AsNoTracking();
            var entity = await entitySet.SingleOrDefaultAsync(e => e.Id == request.Id, token);
            return entity;
        }
        catch (Exception)
        {
            return null;
        }
    }
}