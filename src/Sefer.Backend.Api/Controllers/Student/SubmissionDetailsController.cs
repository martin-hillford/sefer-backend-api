using Sefer.Backend.Api.Views.Student.EnrollmentOverview;
using SubmissionOverview_CorrectedSubmissionResultView = Sefer.Backend.Api.Views.Student.SubmissionOverview.CorrectedSubmissionResultView;
using SubmissionOverview_UncorrectedSubmissionResultView = Sefer.Backend.Api.Views.Student.SubmissionOverview.UncorrectedSubmissionResultView;

namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class SubmissionDetailsController(IServiceProvider provider) : UserController(provider)
{
    private readonly IFileStorageService _fileStorageService = provider.GetService<IFileStorageService>();

    [HttpGet("/student/submissions/{submissionId:int}")]
    [ProducesResponseType(typeof(ISubmissionResultView), 200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ISubmissionResultView>> GetSubmissionDetails(int submissionId)
    {
        // try to load the student that is updating its profile (403)
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        // Retrieve the submission
        var submission = await Send(new GetFinalSubmissionByIdRequest(submissionId));
        if (submission?.Enrollment?.StudentId != student.Id) return NotFound();

        if (submission.Enrollment.IsSelfStudy || submission.ResultsStudentVisible)
        {
            var view = new SubmissionOverview_CorrectedSubmissionResultView(submission, _fileStorageService);
            return Json(view);
        }
        else
        {
            var view = new SubmissionOverview_UncorrectedSubmissionResultView(submission, submission.Enrollment, _fileStorageService);
            return Json(view);
        }
    }
}