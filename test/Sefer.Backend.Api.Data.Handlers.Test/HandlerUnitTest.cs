namespace Sefer.Backend.Api.Data.Handlers.Test;

public abstract class HandlerUnitTest : IDataContextProvider
{
    protected const string MaxLength =
        "this-is-an-incredible-long-string-with-more-than-two-hundred-fifity-five-characters-but-just-that-phrase-is-really-not-enough-it-should-be-much-longer-it-takes-this-lenght-to-top-that-but-even-this-is-not-long-enough-more-is-needed-much-more-we-made-it-almost-yes-we-did";

    private DbContextOptions? _options;

    [TestCleanup]
    public void CleanupCode()
    {
        _options = null;
    }

    protected IServiceProvider ServiceProvider => GetServiceProvider().Object;

    protected virtual MockedServiceProvider GetServiceProvider()
    {
        var provider = new MockedServiceProvider();
        provider.AddDataContextProvider(this);
        return provider;
    }

    protected static MockedServiceProvider GetServiceProvider(Exception exception)
    {
        var provider = new MockedServiceProvider();
        var dataContextProvider = new DataContextExceptionProvider(exception);
        provider.AddDataContextProvider(dataContextProvider);
        return provider;
    }

    protected DataContext GetDataContext()
    {
        if (_options != null) return new DataContext(_options);

        _options = CreateOptions<DataContext>();
        var context = new DataContext(_options);
        context.Database.EnsureCreated();
        return context;
    }

    private static DbContextOptions CreateOptions<T>() where T : DbContext
    {
        // This creates the SQLite connection string to in-memory database
        var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        var connectionString = connectionStringBuilder.ToString();

        // This creates a SqliteConnection with that string
        var connection = new SqliteConnection(connectionString);

        // The connection MUST be opened here
        connection.Open();

        // Now we have the EF Core commands to create SQLite options
        var builder = new DbContextOptionsBuilder<T>();
        builder.UseSqlite(connection);

        return builder.Options;
    }

    protected async Task InsertAsync<T>(T entity)
    {
        var context = GetDataContext();
        await InsertAsync(context, entity);
    }

    protected static async Task InsertAsync<T>(DataContext context, T entity)
    {
        Assert.IsNotNull(entity);
        await context.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    protected async Task InsertAsync<T>(params T[] entities) where T : class
    {
        var context = GetDataContext();
        await InsertAsync(context, entities);
    }

    protected static async Task InsertAsync<T>(DataContext context, params T[] entities) where T : class
    {
        await context.Set<T>().AddRangeAsync(entities);
        await context.SaveChangesAsync();
    }

    public DataContext GetContext() => GetDataContext();
}