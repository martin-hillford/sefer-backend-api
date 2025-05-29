namespace Sefer.Backend.Api.Data.Requests.Entities;

public abstract class UpdateSingleEntityPropertyRequest<T>(T entity, string property) : IRequest<bool>
    where T : class, IEntity
{
    public readonly T Entity = entity;

    public readonly string Property = property;
}