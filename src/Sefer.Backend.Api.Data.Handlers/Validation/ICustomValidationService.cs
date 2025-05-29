namespace Sefer.Backend.Api.Data.Handlers.Validation;

public interface ICustomValidationService<in TEntity> : ICustomValidationService
{
    public Task<bool> IsValid(TEntity instance);
}

// This interface is used for reflection
public interface ICustomValidationService { }