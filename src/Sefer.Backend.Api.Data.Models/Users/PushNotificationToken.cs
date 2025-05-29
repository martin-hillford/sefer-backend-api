namespace Sefer.Backend.Api.Data.Models.Users;

[Keyless]
public class PushNotificationToken
{
    public int UserId { get; set; }

    [MaxLength(int.MaxValue)]
    public string Token { get; set; }
}