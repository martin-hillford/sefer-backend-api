// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Resources;

/// <summary>
/// Object definition for the view
/// </summary>
public class ContentPageOverride
{
    public int Id { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? ModificationDate { get; set; }

    public ContentPageType Type { get; set; }

    public int? SequenceId { get; set; }

    public bool IsPublished { get; set; }

    [MinLength(3), MaxLength(255), Required]
    public string Name { get; set; }

    [MaxLength(int.MaxValue)]
    public string Permalink { get; set; }

    [MaxLength(int.MaxValue)]
    public string Content { get; set; }

    [MaxLength(int.MaxValue)]
    public string Site { get; set; }

    public int SpecificContentId { get; set; }

    public bool IsHtmlContent { get; set; } = true;

    public ContentPage AsContentPage()
    {
        return new ContentPage
        {
            Content = Content,
            CreationDate = CreationDate,
            Id = Id,
            IsHtmlContent = IsHtmlContent,
            IsPublished = IsPublished,
            ModificationDate = ModificationDate,
            Name = Name,
            Permalink = Permalink,
            SequenceId = SequenceId,
            Type = Type,
        };
    }
}