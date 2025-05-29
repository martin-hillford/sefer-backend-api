// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Users;

public class ChangePrimarySitePostModel
{
    public int UserId { get; set; }

    public string Region { get; set; }

    public string Site { get; set; }
}