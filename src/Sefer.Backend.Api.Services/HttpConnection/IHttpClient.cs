namespace Sefer.Backend.Api.Services.HttpConnection;

public interface IHttpClient
{
    public Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string requestUri, TValue value, JsonSerializerOptions options = null, CancellationToken cancellationToken = default);

    // ReSharper disable once UnusedMember.Global
    public Task<T> GetJsonAsync<T>(string requestUri);
}
