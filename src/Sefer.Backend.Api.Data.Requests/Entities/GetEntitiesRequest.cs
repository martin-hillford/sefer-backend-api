namespace Sefer.Backend.Api.Data.Requests.Entities;

public abstract class GetEntitiesRequest<T> : IRequest<List<T>> where T : class {  }