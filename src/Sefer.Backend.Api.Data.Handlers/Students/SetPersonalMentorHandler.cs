namespace Sefer.Backend.Api.Data.Handlers.Students;

public class SetPersonalMentorHandler(IServiceProvider serviceProvider) : Handler<SetPersonalMentorRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(SetPersonalMentorRequest request, CancellationToken token)
    {
        // Check if the student and mentor in the request are valid
        var student = await Send(new GetUserByIdRequest(request.StudentId), token);
        var mentor = await Send(new GetUserByIdRequest(request.MentorId), token);

        if (student == null || !student.IsStudent) return false;
        if (mentor == null || !mentor.IsMentor) return false;

        // check if there are already settings for the student set
        try
        {
            var context = GetDataContext();
            var settings = await context.StudentSettings
                .FirstOrDefaultAsync(x => x.StudentId == request.StudentId, token);

            if (settings != null) settings.PersonalMentorId = request.MentorId;
            else
            {
                settings = new StudentSettings
                {
                    StudentId = request.StudentId,
                    PersonalMentorId = request.MentorId
                };
                context.StudentSettings.Add(settings);
            }

            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception) { return false; }
    }
}