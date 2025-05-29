namespace Sefer.Backend.Api.Data.Handlers.Entities;

public abstract class EntityHandler<TRequest, TEntity, TReturn>(IServiceProvider serviceProvider)
    : Handler<TRequest, TReturn>(serviceProvider)
    where TEntity : class, IEntity
    where TRequest : IRequest<TReturn>
{
    protected async Task<bool> IsValid(TEntity instance) => await IsValidAsync(instance);
}