namespace Sefer.Backend.Api.Services.Avatars;

public static class Extensions
{
    internal static async Task<string> DownloadBlobToStringAsync(this BlobClient blobClient)
    {
        BlobDownloadResult downloadResult = await blobClient.DownloadContentAsync();
        return downloadResult.Content.ToString();
    }

    public static IActionResult Send(this Response response)
    {
        if (response is not { HasImage: true }) return new NotFoundResult();
        if (!response.IsBase64) return new ContentResult { Content = response.Content, ContentType = response.ContentType, StatusCode = 200 };
        var contents = Convert.FromBase64String(response.Base64);
        return new FileContentResult(contents, response.ContentType);
    }
}
