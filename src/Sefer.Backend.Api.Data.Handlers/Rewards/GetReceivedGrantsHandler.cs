namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class GetReceivedGrantsHandler(IServiceProvider serviceProvider)
    : Handler<GetReceivedGrantsRequest, List<RewardGrant>>(serviceProvider)
{
    public override async Task<List<RewardGrant>> Handle(GetReceivedGrantsRequest request, CancellationToken token)
    {
        var student = await Send(new GetUserByIdRequest(request.StudentId), token);
        if (student == null) return new List<RewardGrant>();

        await using var context = GetDataContext();
        return await context.RewardGrants
            .AsNoTracking()
            .Where(r => r.UserId == student.Id)
            .Include(r => r.Reward)
            .OrderByDescending(g => g.Date)
            .ToListAsync(token);
    }
}