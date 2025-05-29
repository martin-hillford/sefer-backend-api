namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class AddMentorRatingHandler(IServiceProvider serviceProvider)
    : Handler<AddMentorRatingRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(AddMentorRatingRequest request, CancellationToken token)
    {
        if (request.MentorRating == null) return false;

        if (request.MentorRating.Rating > 10 || request.MentorRating.Id != 0) return false;

        var mentor = await Send(new GetUserByIdRequest(request.MentorRating.MentorId), token);
        if (mentor == null || mentor.IsMentor == false) return false;

        request.MentorRating.CreationDate = DateTime.UtcNow;

        await using var context = GetDataContext();
        context.MentorRatings.Add(request.MentorRating);
        await context.SaveChangesAsync(token);
        return true;
    }
}