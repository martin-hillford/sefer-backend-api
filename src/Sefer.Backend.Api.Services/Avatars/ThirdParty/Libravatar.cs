using Sefer.Backend.Api.Services.HttpConnection;

namespace Sefer.Backend.Api.Services.Avatars.ThirdParty;

public class Libravatar(IHttpClient client)
{
    public async Task<Response> Retrieve(string hash, int size)
    {
        try
        {
            var uri = $"https://seccdn.libravatar.org/avatar/{hash}?s={size}&forcedefault=y&default=404";
            return await Support.DownloadImageAsync(client, uri);
        }
        catch (Exception)
        {
            return null;
        }
    }
}


