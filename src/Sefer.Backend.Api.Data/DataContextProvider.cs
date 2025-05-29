namespace Sefer.Backend.Api.Data;

/// <summary>
/// A Provider to get the correct data context
/// </summary>
/// <inheritdoc />
public class DataContextProvider : IDataContextProvider
{
    /// <summary>
    /// The options
    /// </summary>
    private readonly DbContextOptions _options;

    /// <summary>
    /// Create a new provider
    /// </summary>
    /// <param name="options"></param>
    public DataContextProvider(DbContextOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Returns a new base context
    /// </summary>
    /// <returns></returns>
    /// <inheritdoc />
    public DataContext GetContext() => new DataContext(_options);
}