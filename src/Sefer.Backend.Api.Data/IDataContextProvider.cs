namespace Sefer.Backend.Api.Data;

/// <summary>
/// A generic interface to get a data context
/// </summary>
public interface IDataContextProvider
{
    /// <summary>
    /// Returns a new data context
    /// </summary>
    /// <returns></returns>
    DataContext GetContext();
}