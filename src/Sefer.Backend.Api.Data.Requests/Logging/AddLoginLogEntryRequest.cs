namespace Sefer.Backend.Api.Data.Requests.Logging;

public class AddLoginLogEntryRequest(LoginLogEntry loginLogEntry) : IRequest<bool>
{
    public readonly LoginLogEntry LoginLogEntry = loginLogEntry;
}