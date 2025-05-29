namespace Sefer.Backend.Api.Data.Handlers.Entities;

public class AsyncDataContext(IServiceProvider serviceProvider)
{
    public DataContext GetDataContext()
    {

        var dataContextProvider = serviceProvider.GetService<IDataContextProvider>();
        return dataContextProvider.GetContext();
    }

    public bool Delete<T>(int? entityId, CancellationToken token) where T : class, IEntity
    {
        if (entityId == null) return false;
        var context = GetDataContext();
        var entitySet = context.Set<T>();

        var existing = entitySet.SingleOrDefault(e => e.Id == entityId);
        if (existing == null) return false;


        entitySet.Remove(existing);
        context.SaveChanges();
        return true;
    }

    public Task<bool> Exists<T>(T instance, CancellationToken token) where T : class, IEntity
        => Exists<T>(instance.Id, token);

    private async Task<bool> Exists<T>(int instanceId, CancellationToken token) where T : class, IEntity
    {
        var context = GetDataContext();
        var entitySet = context.Set<T>();
        var exist = await entitySet.AnyAsync(e => e.Id == instanceId, token);
        await context.DisposeAsync();
        return exist;
    }

    public async Task<bool> InsertOrUpdate<T>(T instance, CancellationToken token) where T : class, IEntity
    {
        var exist = await Exists<T>(instance.Id, token);
        if (exist) return await UpdateAsync(instance, token);
        return await AddAsync(instance, token);
    }

    public async Task<bool> AddAsync<T>(T instance, CancellationToken token) where T : class, IEntity
    {
        try
        {
            if (!await IsValid(instance)) return false;

            if (instance is IModifyDateLogEntity modifyDateLogEntity)
            {
                modifyDateLogEntity.CreationDate = DateTime.Now;
            }

            var context = GetDataContext();
            instance.Id = 0;

            var entitySet = context.Set<T>();
            entitySet.Add(instance);
            await context.SaveChangesAsync(token);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync<T>(T instance, CancellationToken token) where T : class, IEntity
    {
        try
        {
            if (!await IsValid(instance)) return false;

            await using var context = GetDataContext();
            var entitySet = context.Set<T>();

            var existing = await entitySet.SingleOrDefaultAsync(e => e.Id == instance.Id, token);
            if (existing == null) return false;

            if (instance is IModifyDateLogEntity modifyDateLogEntity)
            {
                modifyDateLogEntity.ModificationDate = DateTime.Now;
            }

            context.Entry(existing).CurrentValues.SetValues(instance);
            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> IsValid<TEntity>(TEntity instance)
    {
        if (instance == null) return false;
        var valid = IsValidWithoutCustomValidation(instance);
        if (!valid) return false;

        // Check if there are any additional validation services. If this is not the case
        // then the instance is valid! else used the service to validate the entity
        var service = serviceProvider.GetService<ICustomValidationService<TEntity>>();
        if (service == null) return true;
        return await service.IsValid(instance);
    }

    private bool IsValidWithoutCustomValidation<TEntity>(TEntity instance)
    {

        var context = new ValidationContext(instance, serviceProvider, null);
        var results = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(instance, context, results, true);


        return valid;
    }
}