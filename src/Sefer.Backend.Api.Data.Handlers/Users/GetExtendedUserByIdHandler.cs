namespace Sefer.Backend.Api.Data.Handlers.Users;

public class GetExtendedUserByIdHandler(IServiceProvider serviceProvider)
    : Handler<GetExtendedUserByIdRequest, ExtendedUser>(serviceProvider)
{
    public override async Task<ExtendedUser> Handle(GetExtendedUserByIdRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == request.UserId, token);
        if(user == null) return null;

        var extended = new ExtendedUser { User = user };
        
        var settings = await Send(new GetSettingsRequest(), token);
        var date = DateTime.UtcNow.Date.AddDays(-1 * settings.StudentActiveDays);
        
        extended.LastActivity = await context.UserLastActivities.AsNoTracking().FirstOrDefaultAsync(e => e.UserId == user.Id, token);
        extended.IsActive = extended.LastActivity?.ActivityDate != null && extended.LastActivity.ActivityDate.Date >= date;

        if (user.IsMentor)
        {
            extended.MentorPerformance = await context.MentorPerformance.SingleOrDefaultAsync(p => p.MentorId == user.Id, token);
            extended.MentorSettings = await context.MentorSettings.SingleOrDefaultAsync(s => s.MentorId == user.Id, token);
            extended.MentorActiveStudents = await GetActiveStudents(context, user, date, token);            
        
        }

        return extended;
    }

    private async Task<long?> GetActiveStudents(DataContext context, User mentor, DateTime activeDate, CancellationToken token)
    {
        if(!mentor.IsMentor) return null;
        
        // ReSharper disable once InvertIf
        if (context.Database.IsSqlCapableServer())
        {
            var activeStudents = await context.Set<ActiveStudentsPerMentor>()
                .SingleOrDefaultAsync(c => c.MentorId == mentor.Id, token);
            return activeStudents?.ActiveStudents;
        }

        return await context.UserLastActivities
            .Join(context.Enrollments, 
                a => a.UserId, e => e.StudentId,
                (a, e) => new { e.MentorId, e.StudentId, a.ActivityDate, e.ClosureDate }
                )
            .CountAsync(e => e.ClosureDate == null && e.MentorId == mentor.Id && e.ActivityDate >= activeDate, token);
    }
}