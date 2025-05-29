namespace Sefer.Backend.Api.Data.Models.Resources;

public class ShortUrl
{
    [Key, Required, MaxLength(int.MaxValue), DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; }

    [MaxLength(int.MaxValue)]
    public string Destination { get; set; }

    [MaxLength(int.MaxValue)]
    public string Fallback { get; set; }

    public DateTime? Expires { get; set; }

    public bool IsExpired => Expires.HasValue && Expires.Value < DateTime.UtcNow;
}