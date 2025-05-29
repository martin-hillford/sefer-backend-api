namespace Sefer.Backend.Api.Data.Handlers.Entities;

public static class DataContextExtensions
{
    public static bool Insert<T>(this DataContext context, IServiceProvider provider, T entity)
    {
        try
        {
            if (!BaseValidation.IsValidEntity(provider, entity)) return false;
            context.Add(entity);
            context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }

    public static bool Update<T>(this DataContext context, IServiceProvider provider, T entity)
    {
        if (!BaseValidation.IsValidEntity(provider, entity)) return false;
        context.Update(entity);
        context.SaveChanges();
        return true;
    }
}