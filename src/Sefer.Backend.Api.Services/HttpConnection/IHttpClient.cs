namespace Sefer.Backend.Api.Services.HttpConnection;

public interface IHttpClient
{
    public Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string requestUri, TValue value, JsonSerializerOptions options = null, CancellationToken cancellationToken = default);

    // ReSharper disable once UnusedMember.Global
    public Task<T> GetJsonAsync<T>(string requestUri);
    
    public Task<string> GetStringAsync(string requestUri, HttpClientOptions options = null, CancellationToken cancellationToken = default);

    
    public Task<HttpResponseMessage> GetAsync(string requestUri, HttpClientOptions options = null, CancellationToken cancellationToken = default);
}

public class HttpClientOptions
{
    public string UserAgent { get; init; }

    public void SetupClient(HttpClient client)
    {
        if(UserAgent.IsNotNullOrEmpty()) client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
    }
}