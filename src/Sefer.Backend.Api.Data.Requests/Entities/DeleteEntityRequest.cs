namespace Sefer.Backend.Api.Data.Requests.Entities;

public abstract class DeleteEntityRequest<T>(T entity) : IRequest<bool> where T : class, IEntity
{
    public readonly int? EntityId = entity?.Id;
}