// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Users;

public class ChangeRolePostModel
{
    public int UserId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRoles Role { get; set; }
}