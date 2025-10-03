// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Users;

public class AvatarPostModel
{
    /// <summary>
    /// A base64 image is expected
    /// </summary>
    [Required, MinLength(1)]
    public string Image { get; set; }

    /// <summary>
    /// The content type of the image
    /// </summary>
    [Required, MinLength(1)]
    public string Type { get; set; }
}