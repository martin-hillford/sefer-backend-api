namespace Sefer.Backend.Api.Data.Handlers.Students;

public class GetStudentTierHandler(IServiceProvider serviceProvider)
    : Handler<GetStudentTierRequest, StudentTier>(serviceProvider)
{
    public override async Task<StudentTier> Handle(GetStudentTierRequest request, CancellationToken token)
    {
        // Get the number of students that have completed a given number of lessons, Basically,
        // compute for each student how many lessons he has completed and next, compute for that many
        // lessons how many students has the same number of lessons completed
        await using var context = GetDataContext();
        var completed = await context.LessonSubmissions
            .Where(e => e.IsFinal && e.SubmissionDate >= request.StartDate && e.SubmissionDate <= request.EndDate)
            .GroupBy(e => e.Enrollment.StudentId)
            .Select(g => new { StudentId = g.Key, Completed = g.Count() })
            .GroupBy(s => s.Completed)
            .Select(g => new { Completed = g.Key, Students = g.Count() })
            .ToListAsync(token);
        
        // Next compute how many courses the student has completed
        var current = await context.LessonSubmissions
                .CountAsync(e => e.IsFinal && e.SubmissionDate >= request.StartDate && e.SubmissionDate <= request.EndDate && e.Enrollment.StudentId == request.StudentId, token);
        
        // Also get the total number of student since completed excludes student that do not have completed any lessons
        var totalStudents = await context.Users.CountAsync(c => c.Role == UserRoles.User || c.Role == UserRoles.Student, token);
        var withCompletedLessons = completed.Sum(e => e.Students);
        completed.Add(new { Completed = 0, Students = totalStudents - withCompletedLessons  });
        
        // Now we have to determine how many students are doing better and how many are doing worse
        var result = new StudentTier { TotalStudentCount = totalStudents, CompletedLessonByStudentCount = current };
        foreach (var data in completed)
        {
            if(data.Completed < current) result.StudentCountWithLessCompletedLessons += data.Students;
            if(data.Completed == current) result.StudentCountWithEqualCompletedLessons = data.Students;
            if(data.Completed > current) result.StudentCountWithMoreCompletedLessons += data.Students;
        }
        return result;
    }
}