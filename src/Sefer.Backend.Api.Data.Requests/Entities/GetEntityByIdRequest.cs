namespace Sefer.Backend.Api.Data.Requests.Entities;

public abstract class GetEntityByIdRequest<T>(int? id) : IRequest<T> where T : class, IEntity
{
    public readonly int? Id = id;
}