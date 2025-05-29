namespace Sefer.Backend.Api.Data.Handlers.Entities;

public class GetEntitiesHandler<TRequest, TEntity>(IServiceProvider serviceProvider)
    : Handler<TRequest, List<TEntity>>(serviceProvider) where TRequest : GetEntitiesRequest<TEntity> where TEntity : class
{
    public override async Task<List<TEntity>> Handle(TRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var entitySet = context.Set<TEntity>().AsNoTracking();
        var entities = await entitySet.ToListAsync(token);
        return entities;
    }
}