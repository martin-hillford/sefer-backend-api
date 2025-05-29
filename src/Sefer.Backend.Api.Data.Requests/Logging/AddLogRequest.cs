namespace Sefer.Backend.Api.Data.Requests.Logging;

public class AddLogRequest(Log log) : IRequest<bool>
{
    public readonly Log Log = log;
}