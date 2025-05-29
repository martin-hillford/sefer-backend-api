using System.Net.Http.Headers;
using System.Web;

namespace Sefer.Backend.Api.Services.Newsletter.CleverReach;

public class NewsletterService(IOptions<CleverReachOptions> options, IHttpClientFactory httpClientFactory) : INewsletterService
{
    private readonly CleverReachOptions _options = options.Value;

    public async Task<bool> Subscribe(string name, string email)
    {
        // Get the access token to connect with clever reach
        var accessToken = await GetAccessToken();

        // Create a client to post the data
        var client = httpClientFactory.CreateClient();

        // create the url to post to
        var subscribeUrl = $"https://rest.cleverreach.com/v3/groups.json/{_options.GroupId}/receivers?token={accessToken}";

        // Create the data to subscribe
        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var postBody = new
        {
            email,
            registered = time,
            activated = time,
            deactivated = "0",
            source = _options.Source,
            global_attributes = new { name },
            attributes = new { name },
            client_id = _options.ClientId,
            client_secret = _options.ClientSecret,
            grant_type = "authorization_code",
            code = accessToken
        };

        var response = await client.PostAsJsonAsync(subscribeUrl, postBody);
        var content = await response.Content.ReadAsStringAsync();
        if (content.Contains("Bad Request: duplicate address ")) return true;
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> IsSubscribed(string email)
    {
        // create the url to post to
        var accessToken = await GetAccessToken();
        var receiver = HttpUtility.UrlEncode(email);
        var subscriptionUrl = $"https://rest.cleverreach.com/v3/receivers.json/{receiver}?group_id={_options.GroupId}&token={accessToken}";

        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync(subscriptionUrl);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UnSubscribe(string email)
    {
        // create the url to post to
        var accessToken = await GetAccessToken();
        var receiver = HttpUtility.UrlEncode(email);
        var unsubscribeUrl = $"https://rest.cleverreach.com/v3/receivers.json/{receiver}?group_id={_options.GroupId}&token={accessToken}";

        var client = httpClientFactory.CreateClient();
        var response = await client.DeleteAsync(unsubscribeUrl);
        return response.IsSuccessStatusCode;
    }


    private async Task<string> GetAccessToken()
    {
        // Create an http client to make the request to clever reach
        var client = httpClientFactory.CreateClient();
        const string tokenUrl = "https://rest.cleverreach.com/oauth/token.php";

        // create the request
        var authenticationString = $"{_options.ClientId}:{_options.ClientSecret}";
        var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);
        var data = new { grant_type = "client_credentials" };

        // post and get the data
        var response = await client.PostAsJsonAsync(tokenUrl, data);
        var content = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return content.access_token;
    }
}