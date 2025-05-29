namespace Sefer.Backend.Api.Data.Handlers;

public abstract class Handler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected readonly IServiceProvider ServiceProvider;

    protected readonly IMediator Mediator;

    protected readonly IMemoryCache Cache;

    protected Handler(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Mediator = ServiceProvider.GetService<IMediator>();
        Cache = ServiceProvider.GetService<IMemoryCache>();
    }

    protected DataContext GetDataContext()
    {
        var provider = ServiceProvider.GetService<IDataContextProvider>();
        return provider.GetContext();
    }

    protected AsyncDataContext GetAsyncContext() => new(ServiceProvider);

    protected async Task<bool> IsValidAsync<TEntity>(TEntity instance)
    {
        var context = GetAsyncContext();
        return await context.IsValid(instance);
    }

    protected bool IsValidEntity<TEntity>(TEntity instance)
    {

        var valid = BaseValidation.IsValidEntity(ServiceProvider, instance);

        return valid;
    }

    protected Task<T> Send<T>(IRequest<T> request, CancellationToken cancellationToken = default)
        => Mediator.Send(request, cancellationToken);

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken token);
}

public abstract class SyncHandler<TRequest, TResponse>(IServiceProvider serviceProvider)
    : Handler<TRequest, TResponse>(serviceProvider)
    where TRequest : IRequest<TResponse>
{
    public override Task<TResponse> Handle(TRequest request, CancellationToken token)
    {
        var result = Handle(request);
        return Task.FromResult(result);
    }

    protected abstract TResponse Handle(TRequest request);
}