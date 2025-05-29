namespace Sefer.Backend.Api.Data.Handlers.Resources;

public class InsertShortUrlHandler(IServiceProvider serviceProvider) : Handler<InsertShortUrlRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(InsertShortUrlRequest request, CancellationToken token)
    {
        try
        {
            var valid = await IsValidAsync(request.ShortUrl);
            if (!valid) return false;

            var context = GetDataContext();
            await context.ShortUrls.AddAsync(request.ShortUrl, token);
            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}