// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Users;

/// <summary>
/// Holds a logon
/// </summary>
public class ImpersonationLogonModel
{
    public int UserId { get; set; }

    public string Random { get; set; }

    public string Date { get; set; }

    public string Hash { get; set; }
}