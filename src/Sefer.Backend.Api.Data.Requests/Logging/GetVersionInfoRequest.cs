namespace Sefer.Backend.Api.Data.Requests.Logging;

public class GetVersionInfoRequest : IRequest<VersionInfo>
{
    public bool IsDevelopmentEnv { get; set; }
}