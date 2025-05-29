namespace Sefer.Backend.Api.Data.Models.Resources;

public class SiteSpecificContentPage : ModifyDateLogEntity
{
    [MaxLength(255)]
    public string Site { get; set; }

    public int ContentPageId { get; set; }

    [ForeignKey("ContentPageId")]
    public ContentPage ContentPage { get; set; }

    [MaxLength(int.MaxValue)]
    public string Content { get; set; }

    public bool IsPublished { get; set; }
}