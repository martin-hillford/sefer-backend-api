// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Config;

public class NotificationPostModel
{
    [Required, MinLength(1)]
    public string Title { get; init; }
    
    [Required, MinLength(1)]
    public string Content { get; init; }
    
    [Required]
    public int UserId { get; init; }
}