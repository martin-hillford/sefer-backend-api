namespace Sefer.Backend.Api.Data.Handlers.Entities;

public abstract class UpdateSingleEntityPropertyHandler<TRequest, TEntity>(IServiceProvider serviceProvider)
    : EntityHandler<TRequest, TEntity, bool>(serviceProvider)
    where TRequest : UpdateSingleEntityPropertyRequest<TEntity>
    where TEntity : class, IEntity
{
    public override async Task<bool> Handle(TRequest request, CancellationToken token)
    {
        try
        {
            if (request.Entity == null) return false;
            if (!await IsValid(request.Entity)) return false;
            var context = GetDataContext();
            context.UpdateSingleProperty(request.Entity, request.Property);
        }
        catch (Exception) { return false; }
        return true;
    }
}

