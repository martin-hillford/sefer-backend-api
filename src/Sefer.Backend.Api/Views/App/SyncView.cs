namespace Sefer.Backend.Api.Views.App;

/// <summary>
/// This 
/// </summary>
/// <param name="data"></param>
/// <typeparam name="T"></typeparam>
public class SyncView<T>(List<T> data)
{
    [JsonPropertyName("s_dt")]
    public long SyncDate = DateTime.UtcNow.ToUnixTime();

    [JsonPropertyName("data")]
    public readonly List<T> Data = data;
}