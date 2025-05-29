namespace Sefer.Backend.Api.Data.Requests.Entities;

public abstract class AddEntityRequest<T>(T entity) : IRequest<bool>
    where T : class, IEntity
{
    public readonly T Entity = entity;
}