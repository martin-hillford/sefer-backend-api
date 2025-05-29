namespace Sefer.Backend.Api.Data.Models.Resources;

public class NotificationLocalization : Entity
{
    [MaxLength(255)]
    public string Name { get; set; }
    
    [MaxLength(3)]
    public string Language { get; set; }
    
    [MaxLength(int.MaxValue)]
    public string Content { get; set; }
}