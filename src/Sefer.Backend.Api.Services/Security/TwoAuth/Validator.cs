namespace Sefer.Backend.Api.Services.Security.TwoAuth;

/// <summary>
/// This class can be used to validate tokens of the user
/// </summary>
public class Validator
{
    /// <summary>
    /// This is the generator that is used to generate tokens
    /// </summary>
    private readonly Generator _generator;

    /// <summary>
    /// Creates a new TOPT validator
    /// </summary>
    public Validator()
    {
        _generator = new Generator();
    }

    /// <summary>
    /// Validates a given Time Based One Time Password.
    /// </summary>
    /// <param name="accountSecretKey">User's secret key. Same as used to create the setup.</param>
    /// <param name="clientTotp">Number provided by the user which has to be validated.</param>
    /// <param name="timeToleranceInSeconds">Time tolerance in seconds. Default is to accept 60 seconds before and after now.</param>
    /// <returns>True or False if the validation was successful.</returns>
    public bool Validate(string accountSecretKey, uint clientTotp, int timeToleranceInSeconds = 60)
    {
        var codes = _generator.GetValidPasswords(accountSecretKey, TimeSpan.FromSeconds(timeToleranceInSeconds));
        return codes.Any(c => c == clientTotp);
    }
}