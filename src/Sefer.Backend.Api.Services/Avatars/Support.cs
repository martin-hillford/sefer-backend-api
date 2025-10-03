using Sefer.Backend.Api.Services.HttpConnection;

namespace Sefer.Backend.Api.Services.Avatars;

internal static class Support
{
    public static async Task<Response> DownloadImageAsync(IHttpClient client, string imageUri)
    {
        try
        {
            // Check if the image can be downloaded
            var options = new HttpClientOptions { UserAgent = "Mozilla/5.0 (compatible; AcmeInc/1.0)" };
            var response = await client.GetAsync(imageUri, options);
            if (!response.IsSuccessStatusCode) return null;

            // Check if the result from the server is an image
            var contentType = response.Content.Headers.ContentType;
            if (contentType?.MediaType == null || contentType.MediaType.Contains("image")) return null;

            // Return the result
            var data = await response.Content.ReadAsByteArrayAsync();
            var image = Convert.ToBase64String(data);
            return Response.FromBase64(image, contentType.MediaType, DateTime.UtcNow.AddDays(1));
        }
        catch (Exception)
        {
            return null;
        }
    }
}