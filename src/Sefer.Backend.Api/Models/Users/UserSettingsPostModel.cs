namespace Sefer.Backend.Api.Models.Users;

public class UserSettingsPostModel
{
    public Dictionary<string, string> Settings { get; set; } = new();
    
    public int UserId { get; set; }
}