// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Users;

public class AvatarPostModel
{
    [Required, MinLength(1)]
    public string Image { get; set; }

    [Required, MinLength(1)]
    public string Type { get; set; }
}