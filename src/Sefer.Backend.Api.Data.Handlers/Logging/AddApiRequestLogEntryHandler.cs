using Npgsql;

namespace Sefer.Backend.Api.Data.Handlers.Logging;

public class AddApiRequestLogEntryHandler(IServiceProvider serviceProvider)
    : SyncHandler<AddApiRequestLogEntryRequest, bool>(serviceProvider)
{
    protected override bool Handle(AddApiRequestLogEntryRequest request)
    {
        // Entity Framework can be slow for inserting this log entry
        // therefore a direct insert query is used
        var context = GetDataContext();
        const string insert = "INSERT INTO api_request_log_entries (log_time, Path, user_agent, browser_token, accepted_language, processing_time, method, do_not_track) VALUES(@LogTime, @Path, @UserAgent, @BrowserToken, @AcceptedLanguage, @ProcessingTime, @Method, @DoNotTrack)";

        var data = new object[]
        {
            new NpgsqlParameter("@LogTime", request.Entity.LogTime),
            new NpgsqlParameter("@Path", request.Entity.Path),
            new NpgsqlParameter("@UserAgent", request.Entity.UserAgent),
            new NpgsqlParameter("@BrowserToken", request.Entity.BrowserToken),
            new NpgsqlParameter("@AcceptedLanguage", request.Entity.AcceptedLanguage),
            new NpgsqlParameter("@ProcessingTime", request.Entity.ProcessingTime),
            new NpgsqlParameter("@Method", request.Entity.Method),
            new NpgsqlParameter("@DoNotTrack", request.Entity.DoNotTrack),
        };

        context.Database.ExecuteSqlRaw(insert, data);
        return true;
    }
}