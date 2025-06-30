using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Models.Admin.Users;

public class AdminProfileInfoView(User user, List<UserSetting> settings) : UserView(user)
{
    public Dictionary<string, string> Settings => settings.ToDictionary(k => k.KeyName, v => v.Value);   
}