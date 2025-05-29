namespace Sefer.Backend.Api.Data.Handlers.Logging;

public class AddClientPageRequestLogEntryHandler(IServiceProvider serviceProvider)
        : AddEntityHandler<AddClientPageRequestLogEntryRequest, ClientPageRequestLogEntry>(serviceProvider)
{
}