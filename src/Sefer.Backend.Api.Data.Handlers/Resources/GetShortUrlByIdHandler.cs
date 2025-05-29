namespace Sefer.Backend.Api.Data.Handlers.Resources;

public class GetShortUrlByIdHandler(IServiceProvider serviceProvider) : Handler<GetShortUrlByIdRequest, ShortUrl>(serviceProvider)
{
    public override async Task<ShortUrl> Handle(GetShortUrlByIdRequest request, CancellationToken token)
    {
        try
        {
            var context = GetDataContext();
            return await context.ShortUrls.SingleOrDefaultAsync(s => s.Id == request.Id, token);
        }
        catch (Exception)
        {
            return null;
        }
    }
}