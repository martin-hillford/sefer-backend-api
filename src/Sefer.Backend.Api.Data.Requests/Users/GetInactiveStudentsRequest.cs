namespace Sefer.Backend.Api.Data.Requests.Users;

public class GetInactiveStudentsRequest(DateTime activityDate) : IRequest<List<User>>
{
    public readonly DateTime ActivityDate = activityDate;
}