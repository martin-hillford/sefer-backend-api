namespace Sefer.Backend.Api.Data.Requests.Donations;

public class AddDonationRequest(Donation donation) : IRequest<bool>
{
    public readonly Donation Donation = donation;
}