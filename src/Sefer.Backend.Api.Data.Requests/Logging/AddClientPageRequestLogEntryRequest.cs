namespace Sefer.Backend.Api.Data.Requests.Logging;

public class AddClientPageRequestLogEntryRequest(ClientPageRequestLogEntry entity)
    : AddEntityRequest<ClientPageRequestLogEntry>(entity);