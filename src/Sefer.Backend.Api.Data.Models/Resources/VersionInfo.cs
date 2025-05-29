namespace Sefer.Backend.Api.Data.Models.Resources;

public class VersionInfo
{
    public string Build { get; set; }

    public string Database { get; init; }

    public string Provider { get; init; }

    public string Environment { get; set; }

    public bool IsDevelopmentEnv { get; set; }

    public string AdminEmail { get; set; }
}