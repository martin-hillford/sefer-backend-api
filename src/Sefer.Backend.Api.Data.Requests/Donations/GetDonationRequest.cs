namespace Sefer.Backend.Api.Data.Requests.Donations;

public class GetDonationRequest(string id) : IRequest<Donation>
{
    public readonly string Id = id;

    public GetDonationRequest(Guid id) : this(id.ToString()) { }
}