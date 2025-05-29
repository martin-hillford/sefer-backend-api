namespace Sefer.Backend.Api.Data.Handlers.Students;

public class DeleteStudentHandler(IServiceProvider serviceProvider)
    : Handler<DeleteStudentRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(DeleteStudentRequest request, CancellationToken token)
    {
        // First check if a user is present
        var user = await Send(new GetUserByIdRequest(request.StudentId), token);
        if (user == null) return false;

        var deleted = await DeleteStudent(request.StudentId);
        if (deleted) Cache.Remove("database-user-" + request.StudentId);
        return deleted;
    }

    private async Task<bool> DeleteStudent(int studentId)
    {
        var context = GetDataContext();
        await using var transaction = context.BeginTransaction();

        // Get the user itself
        try
        {
            var student = context.Users.SingleOrDefault(u => u.Id == studentId);
            if (student == null || student.IsStudent == false) return false;

            // Deal with the chat messages of the user
            var messages = context.ChatMessages.Where(m => m.SenderId == studentId);
            await context.ChatMessages
                .Where(m => messages.Any(c => c.Id == m.QuotedMessageId))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.QuotedMessageId, default(int?))
                    .SetProperty(b => b.QuotedMessageString, default(string))
                );

            await context.ChatMessages
                .Where(m => messages.Any(c => c.Id == m.Id))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.QuotedMessageId, default(int?))
                    .SetProperty(b => b.QuotedMessageString, default(string))
                );

            await context.ChatMessages.Where(e => e.SenderId == studentId).ExecuteDeleteAsync();
            await context.ChatChannels.Where(e => e.Type == ChannelTypes.Personal && e.Receivers.Any(r => r.UserId == studentId)).ExecuteDeleteAsync();

            // Deal with the lessons, enrollments of the users
            await context.SurveyResults.Where(e => e.Enrollment.StudentId == studentId).ExecuteDeleteAsync();
            await context.RewardGrants.Where(e => e.UserId == studentId).ExecuteDeleteAsync();
            await context.RewardEnrollments.Where(e => e.Enrollment.StudentId == studentId).ExecuteDeleteAsync();
            await context.LessonSubmissions.Where(e => e.Enrollment.StudentId == studentId).ExecuteDeleteAsync();
            await context.Enrollments.Where(e => e.StudentId == studentId).ExecuteDeleteAsync();

            // Remove login entries of the user
            await context.LoginLogEntries.Where(e => e.UserId == studentId).ExecuteDeleteAsync();
            await context.Users.Where(e => e.Id == studentId).ExecuteDeleteAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}