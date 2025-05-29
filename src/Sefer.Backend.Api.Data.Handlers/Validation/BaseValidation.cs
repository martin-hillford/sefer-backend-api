namespace Sefer.Backend.Api.Data.Handlers.Validation;

public class BaseValidation(IServiceProvider serviceProvider)
{
    private static BaseValidation Create(IServiceProvider serviceProvider) => new(serviceProvider);

    public static bool IsValidEntity<TEntity>(IServiceProvider serviceProvider, TEntity instance)
        => Create(serviceProvider).IsValidEntity(instance);

    private bool IsValidEntity<TEntity>(TEntity instance)
    {
        var context = new ValidationContext(instance, serviceProvider, null);
        var results = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(instance, context, results, true);
        return valid;
    }
}