namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorRatingsHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorRatingsRequest, Dictionary<int, Tuple<int, int>>>(serviceProvider)
{
    public override async Task<Dictionary<int, Tuple<int, int>>> Handle(GetMentorRatingsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Users
            .AsNoTracking()
            .Where(m => m.Ratings.Count != 0 && m.Role == request.Role)
            .Select(m =>
                new
                {
                    MentorId = m.Id,
                    Rating = new Tuple<int, int>
                        (
                            m.Ratings.Select(r => (int)r.Rating).Sum() / m.Ratings.Count,
                            m.Ratings.Count
                        )
                })
            .ToDictionaryAsync(m => m.MentorId, m => m.Rating, token);
    }
}