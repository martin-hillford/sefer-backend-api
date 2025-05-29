// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public.Users;

public class SiteRedirectLogonPostModel
{
    public int UserId { get; set; }

    public string Random { get; set; }

    public string Date { get; set; }

    public string Hash { get; set; }
}