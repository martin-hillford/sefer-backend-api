namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class RemoveAsMentorHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        // Setup users
        var student = new User { Role = UserRoles.Student, Name = "Student", PrimaryRegion = "nl", PrimarySite = "test.tld", Gender = Genders.Male, Email = "student@example.tld", YearOfBirth = 1987 };
        var mentor1 = new User { Role = UserRoles.Mentor, Name = "Mentor1", PrimaryRegion = "nl", PrimarySite = "test.tld", Gender = Genders.Male, Email = "mentor1@example.tld", YearOfBirth = 1987 };
        var mentor2 = new User { Role = UserRoles.Mentor, Name = "Mentor2", PrimaryRegion = "nl", PrimarySite = "test.tld", Gender = Genders.Male, Email = "mentor2@example.tld", YearOfBirth = 1987 };

        await InsertAsync(student, mentor1, mentor2);

        // Setup course , course revision and enrollment
        var course = new Course { Name = "test", Description = "description", Permalink = "test", Author = "author" };
        await InsertAsync(course);

        var revision = new CourseRevision { Version = 1, CourseId = course.Id };
        await InsertAsync(revision);

        var enrollment = new Enrollment { MentorId = mentor1.Id, StudentId = student.Id, CourseRevisionId = revision.Id };
        await InsertAsync(enrollment);
    }

    [TestMethod]
    public async Task HandlerTest()
    {
        var context = GetDataContext();
        var mentor1 = await context.Users.FirstOrDefaultAsync(u => u.Name == "Mentor1");
        var enrollments = await context.Enrollments.Include(c => c.CourseRevision).ThenInclude(c => c.Course).Include(e => e.Student).ToListAsync();
        var mentors = await context.Users.Where(u => u.Role == UserRoles.Mentor).ToListAsync();
        var mentor2 = await context.Users.FirstOrDefaultAsync(u => u.Name == "Mentor2");
        var site = new Mock<ISite>();
        site.SetupGet(s => s.RegionId).Returns("nl");
        site.SetupGet(s => s.Hostname).Returns("test.tld");

        Assert.IsNotNull(mentor1);
        Assert.IsNotNull(mentor2);

        var provider = GetServiceProvider().AddCaching();
        provider.AddRequestResult<GetMentorActiveStudentsRequest, List<Enrollment>>(enrollments);
        provider.AddRequestResult<GetCourseMentorsRequest, List<User>>(mentors);
        provider.SetupMentorAssigning(mentor2);
        provider.AddRequestResult<GetSiteByNameRequest, ISite?>(site.Object);
        provider.AddRequestResult<GetSitesRequest, IEnumerable<ISite>>([site.Object]);

        var request = new RemoveAsMentorRequest(mentor1.Id);
        var handler = new RemoveAsMentorHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        Assert.IsTrue(result);
        context = GetDataContext();
        var enrollment = await context.Enrollments.FirstOrDefaultAsync();
        Assert.IsNotNull(enrollment?.MentorId);
        Assert.AreEqual(enrollment.MentorId, mentor2.Id);
    }
}