namespace Sefer.Backend.Api.Data.Handlers.Students;

public class GetPersonalMentorHandler(IServiceProvider serviceProvider)
    : Handler<GetPersonalMentorRequest, User>(serviceProvider)
{
    public override async Task<User> Handle(GetPersonalMentorRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.StudentSettings
            .Where(s => s.StudentId == request.StudentId)
            .Where(s => s.PersonalMentor != null)
            .Select(s => s.PersonalMentor)
            .FirstOrDefaultAsync(token);
    }
}