// ReSharper disable InconsistentNaming
using Sefer.Backend.Api.Controllers.Student;
using Sefer.Backend.Api.Data.Models.Courses;
using Sefer.Backend.Api.Data.Models.Courses.Curricula;
using Sefer.Backend.Api.Data.Models.Courses.Rewards;
using Sefer.Backend.Api.Data.Models.Enrollments;
using Sefer.Backend.Api.Data.Requests.Curricula;
using Sefer.Backend.Api.Data.Requests.Enrollments;
using Sefer.Backend.Api.Data.Requests.Rewards;
using Sefer.Backend.Api.Services.Pdf;

namespace Sefer.Backend.Api.Test.Controllers.Students;

[TestClass]
public class DiplomaControllerTest : AbstractControllerTest
{
    [TestMethod]
    public async Task GetCurriculumDiploma_GrantNull()
    {
        var parameters = new { };
        await GetCurriculumDiploma_NotFound(parameters);
    }
    
    [TestMethod]
    public async Task GetCurriculumDiploma_GrantCodeNotMatching()
    {
        var parameters = new
        {
            RewardGrant = new RewardGrant()
        };
        await GetCurriculumDiploma_NotFound(parameters);
    }
    
    [TestMethod]
    public async Task GetCurriculumDiploma_TargetValueNull()
    {
        var parameters = new
        {
            RewardGrant = new RewardGrant { Code = "B7BBC294-6D2C" }
        };
        await GetCurriculumDiploma_NotFound(parameters);
    }
    
    [TestMethod]
    public async Task GetCurriculumDiploma_StudentNull()
    {
        var parameters = new
        {
            RewardGrant = new RewardGrant { Code = "B7BBC294-6D2C", Target = new RewardTarget { Target = 13 }, TargetValue = 59 },
            Curriculum = new Curriculum { Id = 13 }
        };
        await GetCurriculumDiploma_NotFound(parameters);
    }
    
    [TestMethod]
    public async Task GetCurriculumDiploma_CurriculumNull()
    {
        var parameters = new
        {
            RewardGrant = new RewardGrant { Code = "B7BBC294-6D2C", Target = new RewardTarget { Target = 13 }, TargetValue = 59 },
            Student = new User { Name = "Test User" }
        };
        await GetCurriculumDiploma_NotFound(parameters);
    }
    
    [TestMethod]
    public async Task GetCurriculumDiploma_RevisionNull()
    {
        var parameters = new
        {
            RewardGrant = new RewardGrant { Code = "B7BBC294-6D2C", Target = new RewardTarget { Target = 13 }, TargetValue = 59 },
            Student = new User { Name = "Test User" },
            Curriculum = new Curriculum { Id = 13 }
        };
        await GetCurriculumDiploma_NotFound(parameters);
    }
    
    [TestMethod]
    public async Task GetCurriculumDiploma_Ok()
    {
        var parameters = new
        {
            RewardGrant = new RewardGrant { Code = "B7BBC294-6D2C", Target = new RewardTarget { Target = 13 }, TargetValue = 59 },
            Student = new User { Name = "Test User" },
            Curriculum = new Curriculum { Id = 13 },
            CurriculumRevision = new CurriculumRevision { Id = 17, CurriculumId = 13 },
            Enrollments = new List<Enrollment>(),
            Courses = new List<Course>()
        };

        var provider = GetCurriculumDiploma_GetServiceProvider(parameters);
        var controller = new DiplomaController(provider.Object);

        var result = await controller.GetCurriculumDiploma(51, "B7BBC294-6D2C", "en");
        
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<OkResult>(result);
    }

    private static async Task GetCurriculumDiploma_NotFound(dynamic parameters)
    {
        var provider = GetCurriculumDiploma_GetServiceProvider(parameters);
        var controller = new DiplomaController(provider.Object);

        var result = await controller.GetCurriculumDiploma(51, "B7BBC294-6D2C", "en");
        
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<NotFoundResult>(result);    
    }
    
    private static MockedServiceProvider GetCurriculumDiploma_GetServiceProvider(dynamic parameters)
    {
        var site = Extensions.GetValueOrNull(parameters, "Site") as ISite;
        var region = Extensions.GetValueOrNull(parameters, "Region") as IRegion;
        
        var provider = GetServiceProvider()
            .AddRequestResult<GetGrantByIdRequest, RewardGrant>(parameters, "RewardGrant")
            .AddRequestResult<GetUserByIdRequest, User>(parameters, "Student")
            .AddRequestResult<GetCurriculumByIdRequest, Curriculum>(parameters, "Curriculum")
            .AddRequestResult<GetCurriculumRevisionByIdRequest, CurriculumRevision>(parameters, "CurriculumRevision")
            .AddRequestResult<GetPrimaryRegionAndSiteRequest, (IRegion, ISite)>((region, site))
            .AddRequestResult<GetCoursesByCurriculumRevisionRequest, List<Course>>(parameters, "Courses")
            .AddRequestResult<GetEnrollmentsOfStudentRequest, List<Enrollment>>(parameters, "Enrollments");
        
        var renderService = new Mock<IPdfRenderService>();
        renderService
            .Setup(s => s.Render("diploma","en",It.IsAny<object>(), "diploma.pdf"))
            .ReturnsAsync(new OkResult());
        provider.AddService(renderService);
        
        return provider;
    }
}