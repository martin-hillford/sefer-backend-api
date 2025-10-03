using System.Text.Json.Nodes;
using Sefer.Backend.Api.Services.HttpConnection;

namespace Sefer.Backend.Api.Services.Avatars.ThirdParty;

public class Gravatar(IHttpClient client)
{
    public async Task<Response> Retrieve(string hash, int size)
    {
        try
        {
            var uri = $"https://www.gravatar.com/{hash}.json";
            var options = new HttpClientOptions { UserAgent = "Mozilla/5.0 (compatible; AcmeInc/1.0)" };
            var response = await client.GetStringAsync(uri, options);
            var json = JsonNode.Parse(response);
            var result = json?["entry"]?[0]?["thumbnailUrl"];
            if (result == null) return null;

            var imageUri = $"{result}?size={size}";
            return await Support.DownloadImageAsync(client, imageUri);
        }
        catch (Exception)
        {
            return null;
        }
    }
}