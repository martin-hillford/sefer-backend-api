namespace Sefer.Backend.Api.Data.Requests.Donations;

public class SetDonationStateRequest(string donationId, PaymentStatus status) : IRequest<bool>
{
    public readonly string DonationId = donationId;

    public readonly PaymentStatus Status = status;
}