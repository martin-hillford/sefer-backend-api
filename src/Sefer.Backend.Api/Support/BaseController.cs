using System.IO;
using System.IO.Compression;
using System.Text.Json;
using Sefer.Backend.Api.Data;

namespace Sefer.Backend.Api.Support;

public abstract class BaseController(IServiceProvider serviceProvider) : ControllerBase
{
    protected readonly IMediator Mediator = serviceProvider.GetService<IMediator>();

    private readonly IUserAuthenticationService _userAuthenticationService
        = serviceProvider.GetService<IUserAuthenticationService>();

    protected readonly IServiceProvider ServiceProvider = serviceProvider;

    protected Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
        => Mediator.Send(request, token);

    protected T GetService<T>() => ServiceProvider.GetService<T>();

    protected async Task<User> GetCurrentUser()
    {
        try
        {
            return _userAuthenticationService?.UserId == null
                ? null
                : await Send(new GetUserByIdRequest(_userAuthenticationService.UserId));
        }
        catch (Exception) { return null; }
    }

    protected async Task<User> GetCurrentUserAsync()
    {
        try { return await GetCurrentUser(); }
        catch (Exception) { return null; }
    }

    protected JsonResult Json(object data, int statusCode)
    {
        var result = Json(data);
        result.StatusCode = statusCode;
        return result;
    }

    protected JsonResult Json(object data)
    {
        return new JsonResult(data);
    }

    protected async Task<IFile> UploadFileAsync(IFileStorageService fileStorageService, IFormFile file, string path, bool isPublic)
    {
        // Check if the path translates to a folder, save the file, create a result in JSON and return it
        var folder = await fileStorageService.ResolveDirectoryAsync(path, isPublic);
        if (folder == null) return null;

        // Open the file for adding
        return await folder.AppendAsync(file);
    }

    protected async Task<bool> DeleteFileAsync(IFileStorageService fileStorageService, string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        var file = await fileStorageService.ResolveFileAsync(path);
        if (file == null) return false;
        var deleted = await file.DeleteAsync();
        return deleted;
    }
    
    protected IActionResult DownloadGzippedJson<T>(T model, string fileName)
    {
        // Serialize the object to JSON
        var json = JsonSerializer.Serialize(model, DefaultJsonOptions.GetOptions());

        // Convert JSON string to bytes
        var jsonBytes = Encoding.UTF8.GetBytes(json);

        // Compress the JSON bytes using GZip
        using var compressedStream = new MemoryStream();
        using (var gzip = new GZipStream(compressedStream, CompressionLevel.Optimal, leaveOpen: true))
        {
            gzip.Write(jsonBytes, 0, jsonBytes.Length);
        }
        compressedStream.Position = 0;
        
        return File( compressedStream.ToArray(), "application/gzip", fileName );
    }
}