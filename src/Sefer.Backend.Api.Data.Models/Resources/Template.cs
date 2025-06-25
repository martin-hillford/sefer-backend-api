namespace Sefer.Backend.Api.Data.Models.Resources;

public class Template : Entity
{
    [MaxLength(255)]
    public string Name { get; set; }
    
    [MaxLength(255)]
    public string LayoutName { get; set; }
    
    [MaxLength(3)]
    public string Language { get; set; }
    
    [MaxLength(255)]
    public string Title { get; set; }
    
    [MaxLength(4), Required, MinLength(1)]
    public string Type { get; set; }
    
    [MaxLength(int.MaxValue)]
    public string Content { get; set; }
    
    public bool HasLayout => !string.IsNullOrEmpty(LayoutName);
}