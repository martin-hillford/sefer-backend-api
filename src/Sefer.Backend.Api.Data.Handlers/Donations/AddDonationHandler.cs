namespace Sefer.Backend.Api.Data.Handlers.Donations;

public class AddDonationHandler(IServiceProvider serviceProvider)
    : SyncHandler<AddDonationRequest, bool>(serviceProvider)
{
    protected override bool Handle(AddDonationRequest request)
    {
        try
        {
            var context = GetDataContext();
            context.Donations.Add(request.Donation);
            context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}