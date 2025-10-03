namespace Sefer.Backend.Api.Services.Avatars;

public class Cache(AvatarOptions avatarOptions)
{
    private readonly Storage _storage = new(avatarOptions);

    public async Task<CacheResult> FromCache(string hash)
    {
        try
        {
            var file = $"{hash}.json";
            if (!await _storage.ExistsAsync(file)) return CacheResult.NotCached;
            var response = await _storage.ReadJsonAsync(file);

            if (response == null) return CacheResult.NotCached;
            if (response.Expires.HasValue && response.Expires < DateTime.UtcNow) return CacheResult.NotCached;

            return new CacheResult(response);
        }
        catch (Exception)
        {
            return CacheResult.NotCached;
        }
    }

    public async Task Store(string hash, Response response)
    {
        var file = $"{hash}.json";
        await _storage.WriteJsonAsync(file, response);
    }
}