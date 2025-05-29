namespace Sefer.Backend.Api.Data.Requests.Logging;

public class GetNotificationAndMailLogsRequest(int skip, int take) : IRequest<List<Log>>
{
  public readonly int Skip = skip;

  public readonly int Take = take;
}
