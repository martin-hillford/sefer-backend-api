namespace Sefer.Backend.Api.Services.Avatars;

public class Storage(AvatarOptions avatarOptions)
{
    #region Settings

    private readonly string _store =
       // EnvVar.GetEnvironmentVariable("AVATAR_STORE") ??
        avatarOptions.Store;

    private readonly bool _useBlob = 
        EnvVar.GetEnvironmentVariable("AVATAR_USE_BLOB")?.ToLower() == "true" ||
        avatarOptions.UseBlob;

    #endregion

    #region Public Methods

    public async Task<bool> ExistsAsync(string file)
    {
        if (_useBlob) return await AzureBlobExistsAsync(file);
        return await FileExistsAsync(file);
    }

    public async Task<Response> ReadJsonAsync(string file)
    {
        if (_useBlob) return await AzureBlobReadJsonAsync(file);
        return await FileReadJsonAsync(file);
    }

    public async Task WriteJsonAsync(string file, Response data)
    {
        if (_useBlob) await AzureBlobWriteJsonAsync(file, data);
        else await FileWriteJsonAsync(file, data);
    }

    #endregion

    #region File System Methods

    private Task<bool> FileExistsAsync(string file)
    {
        if (_store == null) throw new Exception("Store not configured");

        var path = Path.Combine(_store, file);
        var exists = File.Exists(path);

        return Task.FromResult(exists);
    }

    private async Task<Response> FileReadJsonAsync(string file)
    {
        if (_store == null) throw new Exception("Store not configured");
        try
        {
            var path = Path.Combine(_store, file);
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<Response>(json);
        }
        catch (Exception) { return null; }
    }

    private async Task FileWriteJsonAsync(string file, Response data)
    {
        if (_store == null) throw new Exception("Store not configured");
        var dataJson = JsonSerializer.Serialize(data);
        var path = Path.Combine(_store, file);
        await File.WriteAllTextAsync(path, dataJson);
    }

    #endregion

    #region Azure Blob Storage Methods

    private async Task<bool> AzureBlobExistsAsync(string blobName)
    {
        if (_store == null) throw new Exception("Store not configured");
        var container = new BlobContainerClient(new Uri(_store));
        var reference = container.GetBlobClient(blobName);
        return await reference.ExistsAsync();
    }

    private async Task<Response> AzureBlobReadJsonAsync(string blobName)
    {
        if (_store == null) throw new Exception("Store not configured");
        try
        {
            var container = new BlobContainerClient(new Uri(_store));
            var reference = container.GetBlobClient(blobName);
            var json = await reference.DownloadBlobToStringAsync();
            return JsonSerializer.Deserialize<Response>(json);
        }
        catch (Exception) { return null; }
    }

    private async Task AzureBlobWriteJsonAsync(string blobName, Response data)
    {
        if (_store == null) throw new Exception("Store not configured");

        var dataJson = JsonSerializer.Serialize(data);
        byte[] byteArray = Encoding.UTF8.GetBytes(dataJson);
        var stream = new MemoryStream(byteArray);

        var container = new BlobContainerClient(new Uri(_store));
        var blob = container.GetBlobClient(blobName);
        await blob.DeleteIfExistsAsync();
        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = "application/json" });

    }

    #endregion
}