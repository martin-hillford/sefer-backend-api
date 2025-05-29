namespace Sefer.Backend.Api.Services.Security.TwoAuth;

/// <summary>
/// The generator is able to generator the current valid passwords
/// </summary>
public class Generator
{
    /// <summary>
    /// Time Based One Time Password is based on epoch time
    /// </summary>
    private readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Generates a valid Time Based One Time Password.
    /// </summary>
    /// <param name="accountSecretKey">User's secret key. Same as used to create the setup.</param>
    /// <returns>Creates a 6 digit one time password.</returns>
    // ReSharper disable once UnusedMember.Global
    public int Generate(string accountSecretKey)
    {
        return Hasher.Hash(accountSecretKey, GetCurrentCounter());
    }

    /// <summary>
    /// Generate a token for the given counter
    /// </summary>
    /// <param name="accountSecretKey"></param>
    /// <param name="counter"></param>
    /// <returns></returns>
    private int Generate(string accountSecretKey, long counter)
    {
        return Hasher.Hash(accountSecretKey, counter);
    }

    /// <summary>
    /// Gets valid valid TOTPs.
    /// </summary>
    /// <param name="accountSecretKey">User's secret key. Same as used to create the setup.</param>
    /// <param name="timeTolerance">Time tolerance in seconds to accept before and after now.</param>
    /// <returns>List of valid totps.</returns>
    public IEnumerable<int> GetValidPasswords(string accountSecretKey, TimeSpan timeTolerance)
    {
        var codes = new List<int>();
        var iterationCounter = GetCurrentCounter();
        var iterationOffset = 0;

        if (timeTolerance.TotalSeconds > 30)
        {
            iterationOffset = Convert.ToInt32(timeTolerance.TotalSeconds / 30.00);
        }

        var iterationStart = iterationCounter - iterationOffset;
        var iterationEnd = iterationCounter + iterationOffset;

        for (var counter = iterationStart; counter <= iterationEnd; counter++)
        {
            codes.Add(Generate(accountSecretKey, counter));
        }

        return codes.ToArray();
    }

    /// <summary>
    /// Gets the number of 30 second intervals since epoch
    /// </summary>
    /// <returns></returns>
    private long GetCurrentCounter()
    {
        return (long)(DateTime.UtcNow - _unixEpoch).TotalSeconds / 30;
    }
}