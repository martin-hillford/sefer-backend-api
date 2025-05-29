namespace Sefer.Backend.Api.Services.HttpConnection;

public class HttpClientWrapper(IHttpClientFactory httpClientFactory) : IHttpClient
{
    public async Task<T> GetJsonAsync<T>(string requestUri)
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync(requestUri);
        if (!response.IsSuccessStatusCode) return default;

        var json = await response.Content.ReadAsStringAsync();
        var options = DefaultJsonOptions.GetOptions();
        return JsonSerializer.Deserialize<T>(json, options);
    }

    public Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string requestUri, TValue value, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient();
        return client.PostAsJsonAsync(requestUri, value, options, cancellationToken);
    }
}
