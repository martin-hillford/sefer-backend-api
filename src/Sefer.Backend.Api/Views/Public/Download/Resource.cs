namespace Sefer.Backend.Api.Views.Public.Download;

public class Resource
{
    public Guid Id { get; init; }
    
    public string OriginalUrl { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Base64 { get; set; }
    
    public string ImageType { get; set; }
    
    public bool External { get; set; }

    public string GetResourceUrl() => $"ref://{Id}";

    public void SetResourceIsExternal()
    {
        External = true;
        Base64 = null;
    }
}