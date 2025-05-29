using Sefer.Backend.Api.Data.Models.Financials;
using Sefer.Backend.Api.Data.Requests.Donations;
using Sefer.Backend.Api.Models.Users;
using Support_PaymentOptions = Sefer.Backend.Api.Support.PaymentOptions;

namespace Sefer.Backend.Api.Controllers.Public;

public class DonationController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly Support_PaymentOptions _paymentOptions = serviceProvider.GetService<IOptions<Support_PaymentOptions>>()?.Value;

    [HttpPost("/donate")]
    public async Task<ActionResult> CreateDonation([FromBody] DonationPromisePostModel promise)
    {
        // If no donation is received, return
        var site = await Send(new GetSiteByNameRequest(promise.Site));
        if (site == null || promise.Amount < 1) return BadRequest();

        // Use mollie to handle the financial transaction
        var internalUniqueId = Guid.NewGuid().ToString();
        var amount = promise.Amount + ".00";
        var paymentClient = new PaymentClient(_paymentOptions.MollieApiKey);
        var paymentRequest = new PaymentRequest
        {
            Amount = new Amount(Currency.EUR, amount),
            Description = promise.Description,
            RedirectUrl = site.SiteUrl + "/donate/feedback/" + internalUniqueId
        };
        var paymentResponse = await paymentClient.CreatePaymentAsync(paymentRequest);
        var checkoutLink = paymentResponse.Links.Checkout?.Href;
        if(string.IsNullOrEmpty(checkoutLink)) return BadRequest();

        // Insert the donation in the database - used for tracking donations
        var donation = new Donation
        {
            Amount = promise.Amount,
            Id = internalUniqueId,
            PaymentId = paymentResponse.Id,
        };
        await Send(new AddDonationRequest(donation));

        // Return the checkout link to the user
        return Json(new { CheckoutLink = checkoutLink });
    }

    [HttpGet("/donation/{donationId:guid}")]
    public async Task<ActionResult> GetDonationResult(Guid donationId)
    {
        var donation = await Send(new GetDonationRequest(donationId));
        if (donation == null) return NotFound();

        if (donation.Status != PaymentStatus.Open) return Json(new { donation.Status });

        var currentStatus = await GetStatus(donation.PaymentId);
        var saved = await Send(new SetDonationStateRequest(donation.Id, currentStatus));
        if (!saved) return BadRequest();

        return Json(new { Status = currentStatus });
    }

    private async Task<PaymentStatus> GetStatus(string paymentId)
    {
        var paymentClient = new PaymentClient(_paymentOptions.MollieApiKey);
        var response = await paymentClient.GetPaymentAsync(paymentId);
        return response.Status.ToLower() switch
        {
            "canceled" => PaymentStatus.Canceled,
            "pending" => PaymentStatus.Pending,
            "authorized" => PaymentStatus.Authorized,
            "expired" => PaymentStatus.Expired,
            "failed" => PaymentStatus.Failed,
            "paid" => PaymentStatus.Paid,
            _ => PaymentStatus.Open
        };
    }
}