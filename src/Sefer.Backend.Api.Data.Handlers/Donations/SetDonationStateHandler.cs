namespace Sefer.Backend.Api.Data.Handlers.Donations;

public class SetDonationStateHandler(IServiceProvider serviceProvider)
    : SyncHandler<SetDonationStateRequest, bool>(serviceProvider)
{
    protected override bool Handle(SetDonationStateRequest request)
    {
        try
        {
            var context = GetDataContext();
            var donation = context.Donations.SingleOrDefault(d => d.Id == request.DonationId);
            if (donation == null) return false;
            donation.Status = request.Status;
            context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}