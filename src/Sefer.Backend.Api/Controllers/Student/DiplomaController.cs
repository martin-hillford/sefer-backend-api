using Sefer.Backend.Api.Data;
using Sefer.Backend.Api.Data.Requests.Curricula;
using Sefer.Backend.Api.Data.Requests.Rewards;
using Sefer.Backend.Api.Models.Pdf;

namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class DiplomaController(IServiceProvider serviceProvider) : UserController(serviceProvider)
{
    private readonly ICryptographyService _cryptographyService = serviceProvider.GetService<ICryptographyService>();

    [AllowAnonymous]
    [HttpGet("/student/enrollments/{enrollmentId:int}/diploma/{hash}/{lang}")]
    public async Task<ActionResult> GetDiploma(int enrollmentId, string hash, string lang, string html)
    {
        // Get the student that made the request
        hash = Convert.ToBase64String(hash.HexToBytes());
        if (_cryptographyService.IsValidHash(hash, enrollmentId.ToString()) == false) return Forbid();

        // Load the enrollment
        var enrollment = await Send(new GetEnrollmentByIdExtensivelyRequest(enrollmentId));
        if (enrollment == null) return NotFound();
        if (enrollment.ClosureDate.HasValue == false || enrollment.IsCourseCompleted == false) return Forbid();

        // Generate the certificate
        var language = SupportedLanguages.GetLanguage(lang);
        var (region, site) = await Send(new GetPrimaryRegionAndSiteRequest(enrollment.Student.Id));
        var model = new CourseCertificateModel(site, region, enrollment, language);
        var view = new PdfView(ServiceProvider);
        
        // Deter
        if (html == "true") return await view.RenderAsHtml("course-certificate", lang, language);
        return await view.Render(HttpContext, "course-certificate", language, model, "certificate.pdf");
    }

    // Please note: Since this link is access directly, it is protected with a hash rather than the usual headers
    [AllowAnonymous]
    [HttpGet("/student/curriculum/{rewardGrantId:int}/diploma/{hash}/{lang}")]
    public async Task<ActionResult> GetCurriculumDiploma(int rewardGrantId, string hash, string lang, string html)
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

        var courses = await Send(new GetCoursesByCurriculumRevisionRequest(revision.Id));
        var request = await Send(new GetEnrollmentsOfStudentRequest(student.Id, null, true));
        var enrollments = request
            .Where(r =>
                r.IsCourseCompleted &&
                r.ClosureDate.HasValue &&
                courses.Select(c => c.Id).Contains(r.CourseRevision.CourseId))
            .ToList();

        // It might happen somebody has taken a course twice
        var lookup = new Dictionary<int, Enrollment>();
        foreach (var enrollment in enrollments)
        {
            var courseId = enrollment.CourseRevision.CourseId;
            // ReSharper disable once CanSimplifyDictionaryLookupWithTryAdd
            if (lookup.ContainsKey(courseId) == false) lookup.Add(courseId, enrollment);
            else if (lookup[courseId].Grade.HasValue == false || lookup[courseId].Grade < enrollment.Grade) lookup[courseId] = enrollment;
        }
        enrollments = lookup.Values.OrderBy(c => c.ClosureDate).ThenBy(c => c.CourseRevision.Course.Name).ToList();

        var model = new CurriculumDiploma
        {
            Curriculum = curriculum,
            Grant = grant,
            Student = student,
            Enrollments = enrollments,
            Site = site,
            Region = region,
            Language = language
        };

        // Generate the diploma
        var view = new PdfView(ServiceProvider);
        if (html == "true") return await view.RenderAsHtml("diploma", language, model);
        return await view.Render(HttpContext, "diploma", language, model, "diploma.pdf");
    }
}