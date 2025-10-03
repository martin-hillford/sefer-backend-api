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

    /// <summary>
    /// Send a GET request to the specified Uri and return the response body as a string in an asynchronous operation.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="options">Specific options for making the request</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task<string> GetStringAsync(string requestUri, HttpClientOptions options = null, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient();
        options?.SetupClient(client);
        return await client.GetStringAsync(requestUri, cancellationToken);
    }

    /// <summary>
    /// Send a GET request to the specified Uri with a cancellation token as an asynchronous operation.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="options">Specific options for making the request</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task<HttpResponseMessage> GetAsync(string requestUri, HttpClientOptions options = null, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient();
        options?.SetupClient(client);
        return await client.GetAsync(requestUri, cancellationToken);
    }

    public Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string requestUri, TValue value, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient();
        options ??= DefaultJsonOptions.GetOptions();
        return client.PostAsJsonAsync(requestUri, value, options, cancellationToken);
    }
}
