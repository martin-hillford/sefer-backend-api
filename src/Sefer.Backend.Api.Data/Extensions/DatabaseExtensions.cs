namespace Sefer.Backend.Api.Data.Extensions;

public static class DatabaseExtensions
{
    public static string GetDatabaseVersion(this DataContext context) => context.GetDatabaseVersion();

    public static string GetDatabaseConnection(this DataContext context) => context.GetDatabaseConnection();

    public static string GetDatabaseProvider(this DataContext context) => context.GetDatabaseProvider();

    public static IDbContextTransaction BeginTransaction(this DataContext context)
        => context.Database.BeginTransaction();

}