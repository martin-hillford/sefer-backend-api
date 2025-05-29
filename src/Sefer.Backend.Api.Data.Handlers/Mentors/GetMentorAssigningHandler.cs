namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorAssigningHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorAssigningRequest, User>(serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override async Task<User> Handle(GetMentorAssigningRequest request, CancellationToken token)
    {
        // The first step is to test if the student has a personal mentor
        var personalMentorRequest = new GetPersonalMentorForCourseRequest(request.StudentId, request.CourseId);
        var personalMentor = await Send(personalMentorRequest, token);
        if (personalMentor != null) return personalMentor;

        // Get the settings and check if a backup mentor is available
        var backupMentor = await Send(new GetBackupMentorRequest(), token);
        var settings = await Send(new GetSettingsRequest(), token);
        if (settings == null || backupMentor == null || backupMentor.IsMentor == false) return null;

        // Now prepare everything for the algorithm
        try
        {
            var student = await Send(new GetUserByIdRequest(request.StudentId), token);
            if (student == null) return null;

            var course = await Send(new GetPublishedCourseByIdRequest(request.CourseId), token);
            if (course == null) return null;

            var lastEnrollment = await Send(new GetLastClosedEnrollmentRequest(student.Id), token);
            var activeStudents = await Send(new GetActiveStudentsOfMentorsRequest(), token);
            var (region, _) = await Send(new GetPrimaryRegionAndSiteRequest(student.Id), token);

            var mentors = await GetMentors(region.Id, request.CourseId, token);
            var input = new MentorAssigningInput
            {
                Mentors = mentors,
                ActiveStudents = activeStudents,
                Student = student,
                WebsiteSettings = settings,
                LastStudentEnrollment = lastEnrollment,
                BackupMentor = backupMentor
            };

            return GetMentor(input);
        }
        catch (Exception) { return backupMentor; }
    }

    private async Task<Dictionary<int, User>> GetMentors(string regionId, int courseId, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.MentorCourses
            .AsNoTracking()
            .Where(m => m.CourseId == courseId)
            .Where(m => m.Mentor.MentorRegions.Any(s => s.RegionId == regionId))
            .Include(m => m.Mentor).ThenInclude(m => m.MentorSettings)
            .Select(m => m.Mentor)
            .ToDictionaryAsync(m => m.Id, token);
    }

    private User GetMentor(MentorAssigningInput input)
    {
        var factory = _serviceProvider.GetService<IMentorAssigningFactory>();
        var assigner = factory.PrepareAlgorithm(input);
        return assigner.GetMentor();
    }
}