// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> PostJsonSync(this HttpClient httpClient, string url, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var content =  new StringContent(json, Encoding.UTF8, "application/json");
        return await httpClient.PostAsync(url, content);
    }
}
