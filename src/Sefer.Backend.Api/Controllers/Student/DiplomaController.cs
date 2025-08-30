using Sefer.Backend.Api.Data;
using Sefer.Backend.Api.Data.Requests.Curricula;
using Sefer.Backend.Api.Data.Requests.Rewards;
using Sefer.Backend.Api.Models.Pdf;
using Sefer.Backend.Api.Services.Pdf;

namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class DiplomaController(IServiceProvider serviceProvider) : UserController(serviceProvider)
{
    private readonly ICryptographyService _cryptographyService = serviceProvider.GetService<ICryptographyService>();
    
    private readonly IPdfRenderService _pdfRenderService = serviceProvider.GetService<IPdfRenderService>();

    [AllowAnonymous]
    [HttpGet("/student/enrollments/{enrollmentId:int}/diploma/{hash}/{lang}")]
    [ResponseCache(Duration = 31536000, Location = ResponseCacheLocation.Client)]
    public async Task<ActionResult> GetDiploma(int enrollmentId, string hash, string lang, [FromQuery] string html = "false")
    {
        // Get the student that made the request
        hash = Convert.ToBase64String(hash.HexToBytes());
        if (!_cryptographyService.IsValidHash(hash, enrollmentId.ToString())) return Forbid();

        // Load the enrollment
        var enrollment = await Send(new GetEnrollmentByIdExtensivelyRequest(enrollmentId));
        if (enrollment == null) return NotFound();
        if (!enrollment.ClosureDate.HasValue || !enrollment.IsCourseCompleted) return Forbid();

        // Generate the certificate
        var language = SupportedLanguages.GetLanguage(lang);
        var (region, site) = await Send(new GetPrimaryRegionAndSiteRequest(enrollment.Student.Id));
        var model = new CourseCertificateModel(site, region, enrollment, language);
        
        // Deter
        if (html == "true") return await _pdfRenderService.RenderAsHtml("course-certificate", lang, language);
        return await _pdfRenderService.Render("course-certificate", language, model, "certificate.pdf");
    }

    // Please note: Since this link is access directly, it is protected with a hash rather than the usual headers
    [AllowAnonymous]
    [HttpGet("/student/curriculum/{rewardGrantId:int}/diploma/{hash}/{lang}")]
    [ResponseCache(Duration = 31536000, Location = ResponseCacheLocation.Client)]
    public async Task<ActionResult> GetCurriculumDiploma(int rewardGrantId, string hash, string lang, [FromQuery] string html = "false")
    {
        // First load the grant
        var language = SupportedLanguages.GetLanguage(lang);
        var grant = await Send(new GetGrantByIdRequest(rewardGrantId));
        if (grant == null || hash != grant.Code || grant.TargetValue == null) return NotFound();

        var student = await Send(new GetUserByIdRequest(grant.UserId));
        var curriculum = await Send(new GetCurriculumByIdRequest(grant.Target.Target));
        var revision = await Send(new GetCurriculumRevisionByIdRequest((int)grant.TargetValue));
        if (student == null || curriculum == null || revision == null) return NotFound();
        var (region, site) = await Send(new GetPrimaryRegionAndSiteRequest(student.Id));

        var enrollments = await GetStudentEnrollments(student.Id, revision.Id);
        var model = new CurriculumDiploma
        {
            Curriculum = curriculum,
            Grant = grant,
            Student = student,
            Enrollments = enrollments.Select(e => new CurriculumDiplomaEnrollment(e)).ToList(),
            Site = site,
            Region = region,
            Language = language
        };

        // Generate the diploma
        if (html == "true") return await _pdfRenderService.RenderAsHtml("diploma", language, model);
        return await _pdfRenderService.Render( "diploma", language, model, "diploma.pdf");
    }
    
    private async Task<List<Enrollment>> GetStudentEnrollments(int studentId, int curriculumId)
    {
        var courses = await Send(new GetCoursesByCurriculumRevisionRequest(curriculumId));
        var request = await Send(new GetEnrollmentsOfStudentRequest(studentId, null, true));
        return request
            .Where(r =>
                r.IsCourseCompleted &&
                r.ClosureDate.HasValue &&
                courses.Select(c => c.Id).Contains(r.CourseRevision.CourseId))
            .GroupBy(r => r.CourseRevision.CourseId)
            .Select(g => g.OrderByDescending(c => c.Grade ?? -1).First())
            .OrderBy(c => c.ClosureDate)
            .ThenBy(c => c.CourseRevision.Course.Name)
            .ToList();
    }
}