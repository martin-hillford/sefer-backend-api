namespace Sefer.Backend.Api.Data.Handlers.Donations;

public class GetDonationHandler(IServiceProvider serviceProvider)
    : Handler<GetDonationRequest, Donation>(serviceProvider)
{
    public override async Task<Donation> Handle(GetDonationRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Donations.SingleOrDefaultAsync(d => d.Id == request.Id, token);
    }
}