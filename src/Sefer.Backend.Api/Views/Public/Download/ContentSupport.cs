using System.IO;
using Sefer.Backend.Api.Models.Public;

namespace Sefer.Backend.Api.Views.Public.Download;

internal static class ContentSupport
{
    internal static List<string> FindImageUrls(string text)
    {
        // Regular expression to match image URLs
        const string pattern = @"(?:https?:\/\/|\/public)[^""\s]+?\.(jpg|jpeg|png|gif|bmp|webp|svg)";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        
        // Find all matches in the text
        var matches = regex.Matches(text);
        
        // Create a list to store the image URLs
        return matches.Select(match => match.Value).ToList();
    }
    
    public static async Task<Resource> CreateResource(DownloadRequest request, string imageUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(imageUrl)) return null;

            var path = new Uri(imageUrl).AbsolutePath;
            var type = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();
            
            if(imageUrl.StartsWith("/public")) imageUrl = request.FileStorageService.GetUrl(imageUrl);
            
            var imageBytes = await GetImageBytes(request, imageUrl);
            var base64 = Convert.ToBase64String(imageBytes);
            return new Resource { Base64 = base64, ImageType = type, OriginalUrl = imageUrl, Id = Guid.NewGuid() };
        }
        catch (Exception) { return null; }
    }
    
    private static async Task<byte[]> GetImageBytes(DownloadRequest request, string imageUrl)
    {
        var client = request.HttpClientFactory.CreateClient();
        var imageBytes = await client.GetByteArrayAsync(imageUrl);
        
        // The assumption is made that the images are uploaded in the best format present (likely avif)
        // but if that is not supported, the image should be converted

        return imageBytes;
    }
}