namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetStudentRemarksHandler(IServiceProvider serviceProvider)
    : Handler<GetStudentRemarksRequest, MentorStudentData>(serviceProvider)
{
    public override async Task<MentorStudentData> Handle(GetStudentRemarksRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.MentorStudentData
            .FirstOrDefaultAsync(d => d.MentorId == request.MentorId && d.StudentId == request.StudentId, token);
    }
}