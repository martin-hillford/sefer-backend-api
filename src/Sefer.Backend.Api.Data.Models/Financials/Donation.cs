// This is an entity framework model so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Financials;

/// <summary>
/// Holds information on a donation
/// </summary>
public class Donation
{
    /// <summary>
    /// A unique id for the payment
    /// </summary>
    [MaxLength(36), Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The date the donation was done
    /// </summary>
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The status of the payment (based on mollie)
    /// </summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Open;

    /// <summary>
    /// The amount the person is donating
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Holds the payment id as communicated by mollie
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string PaymentId { get; set; }
}