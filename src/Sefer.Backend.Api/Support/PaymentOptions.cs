// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Support;

/// <summary>
/// The options with payment information
/// </summary>
public class PaymentOptions
{
    /// <summary>
    /// The api-key to use with mollie
    /// </summary>
    public string MollieApiKey { get; set;}
}