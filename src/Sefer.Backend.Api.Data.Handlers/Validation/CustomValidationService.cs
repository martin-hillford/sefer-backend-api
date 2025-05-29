namespace Sefer.Backend.Api.Data.Handlers.Validation;

public abstract class CustomValidationService<T>(IServiceProvider serviceProvider) : ICustomValidationService<T>
{
    public virtual Task<bool> IsValid(T instance)
    {

        var valid = BaseValidation.IsValidEntity(serviceProvider, instance);

        return Task.FromResult(valid);
    }

    protected DataContext GetDataContext()
    {

        var provider = serviceProvider.GetService<IDataContextProvider>();
        return provider.GetContext();

    }

    protected async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {

        var mediator = serviceProvider.GetService<IMediator>();
        var result = await mediator.Send(request, cancellationToken);

        return result;
    }
}