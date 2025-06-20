namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class RemoveAsMentorHandler(IServiceProvider serviceProvider)
    : Handler<RemoveAsMentorRequest, bool>(serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override async Task<bool> Handle(RemoveAsMentorRequest request, CancellationToken token)
    {
        try
        {
            await ReassignStudents(request.MentorId, token);
            RemoveAsMentorFromCourse(request.MentorId);
            RemoveMentorRegions(request.MentorId);

            Cache.Remove("database-user-" + request.MentorId);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task ReassignStudents(int mentorId, CancellationToken token)
    {
        var enrollments = await Send(new GetMentorActiveStudentsRequest(mentorId), token);
        var settings = await Send(new GetSettingsRequest(), token);
        var activeStudents = await Send(new GetActiveStudentsOfMentorsRequest(), token);
        var backupMentor = await Send(new GetBackupMentorRequest(), token);

        var courseMentors = await GetMentorForCourses(token);
        var sites = (await Send(new GetSitesRequest(), token)).ToList();

        foreach (var enrollment in enrollments)
        {
            if (enrollment.Student == null) continue;

            enrollment.MentorId = null;
            enrollment.Mentor = null;

            // Get the mentors that are available for this course and are in the same region as the student
            var courseId = enrollment.CourseRevision.CourseId;
            var regionId = sites.Single(s => s.Hostname == enrollment.Student.PrimarySite).RegionId;
            var mentors = GetMentors(courseMentors, courseId, regionId, mentorId);

            var input = new MentorAssigningInput
            {
                Student = enrollment.Student,
                Mentors = mentors,
                ActiveStudents = activeStudents,
                WebsiteSettings = settings,
                LastStudentEnrollment = enrollment,
                BackupMentor = backupMentor
            };

            var reassignedMentorId = GetMentor(input).Id;
            UpdateEnrollmentMentor(enrollment.Id, reassignedMentorId);
        }
    }

    private User GetMentor(MentorAssigningInput input)
    {
        var factory = _serviceProvider.GetService<IMentorAssigningFactory>();
        var assigner = factory.PrepareAlgorithm(input);
        return assigner.GetMentor();
    }

    // Todo: see if an optimized data structure can be found to improve performance
    private async Task<List<MentorCourse>> GetMentorForCourses(CancellationToken token)
    {
        var context = GetDataContext();
        return await context.MentorCourses
            .AsTracking()
            .Include(m => m.Mentor).ThenInclude(m => m.MentorCourses)
            .Select(m => m)
            .ToListAsync(token);
    }

    private static Dictionary<int, User> GetMentors(List<MentorCourse> courseMentors, int courseId, string regionId, int excludeMentor)
    {
        return courseMentors
                .Where(c => c.CourseId == courseId && c.MentorId != excludeMentor && c.Mentor.MentorRegions.Any(s => s.RegionId == regionId))
                .Where(m => m.Mentor.MentorSettings.IsPersonalMentor == false)
                .Select(m => m.Mentor)
                .DistinctBy(m => m.Id)
                .ToDictionary(m => m.Id, m => m);
    }

    private void RemoveAsMentorFromCourse(int mentorId)
    {
        var context = GetDataContext();
        var mentorCourses = context.MentorCourses.Where(mc => mc.MentorId == mentorId);
        context.MentorCourses.RemoveRange(mentorCourses);
        context.SaveChanges();
    }

    private void RemoveMentorRegions(int mentorId)
    {
        var context = GetDataContext();
        var mentorRegions = context.MentorRegions.Where(mc => mc.MentorId == mentorId).ToList();
        context.MentorRegions.RemoveRange(mentorRegions);
        context.SaveChanges();
    }

    private void UpdateEnrollmentMentor(int enrollmentId, int mentorId)
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.Single(e => e.Id == enrollmentId);
        enrollment.MentorId = mentorId;
        context.UpdateSingleProperty(enrollment, nameof(enrollment.MentorId));
    }
}