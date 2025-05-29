namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class SaveMentorStudentDataHandler(IServiceProvider serviceProvider)
    : Handler<SaveMentorStudentDataRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(SaveMentorStudentDataRequest request, CancellationToken token)
    {
        try
        {
            // Check if the student is a student of the mentor
            var isStudent = await Send(new IsStudentOfMentorRequest(request.Data.MentorId, request.Data.StudentId), token);
            if (!isStudent) return false;

            // Get or create the data object
            var context = GetDataContext();
            var data = await context.MentorStudentData.FirstOrDefaultAsync(d => d.MentorId == request.Data.MentorId && d.StudentId == request.Data.StudentId, token)
                ?? new MentorStudentData { MentorId = request.Data.MentorId, StudentId = request.Data.StudentId };

            // Fill all properties
            data.Remarks = request.Data.Remarks;

            // Insert or update
            if (data.Id > 0) context.Update(data);
            else context.MentorStudentData.Add(data);
            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception) { return false; }

    }
}

