namespace Sefer.Backend.Api.Data.Handlers.Entities;

public class IsValidEntityHandler<TRequest, TEntity> : Handler<TRequest, bool>
    where TRequest : IsValidEntityRequest<TEntity>
    where TEntity : class, IEntity
{
    protected IsValidEntityHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<bool> Handle(TRequest request, CancellationToken token)
        => await IsValidAsync(request.Entity);
}