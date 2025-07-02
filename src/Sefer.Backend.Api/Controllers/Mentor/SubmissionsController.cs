using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Views.Mentor;
using Sefer.Backend.Api.Views.Shared.Enrollments;

namespace Sefer.Backend.Api.Controllers.Mentor;

[Authorize(Roles = "Mentor")]
public class SubmissionsController(IServiceProvider provider) : ReviewController(provider)
{
    private readonly IFileStorageService _fileStorageService = provider.GetService<IFileStorageService>();

    private readonly IAvatarService _avatarService = provider.GetService<IAvatarService>();

    [HttpGet("/mentor/submissions/{lessonSubmissionId:int}/set-visible")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> OpenQuestionResultToStudent(int lessonSubmissionId)
    {
        // Check if the submission is available
        var submission = await Send(new GetSubmissionByIdRequest(lessonSubmissionId));
        if (submission == null) return NotFound();

        // Check if the user is the mentor of the submission
        var enrollment = await Send(new GetEnrollmentByIdRequest(submission.EnrollmentId));
        if (enrollment == null) return NotFound();
        if (enrollment.MentorId != UserId) return Forbid();
        
        // Update the user took some action
        if(enrollment.MentorId.HasValue) await Send(new UpdateUserLastActivityRequest(enrollment.MentorId.Value));

        // And now update it
        submission.ResultsStudentVisible = true;
        var updated = await Send(new UpdateSingleSubmissionPropertyRequest(submission, "ResultsStudentVisible"));
        return updated ? Accepted() : StatusCode(500);
    }

    [HttpGet("/mentor/submissions/{submissionId:int}")]
    [ProducesResponseType(typeof(MentorCorrectedSubmissionView), 200)]
    public async Task<ActionResult<MentorCorrectedSubmissionView>> GetSubmissionDetails(int submissionId)
    {
        // try to load the student that is updating his profile (403)
        var mentor = await GetCurrentUser();
        if (mentor == null || mentor.IsMentor == false) return Forbid();

        // Retrieve the submission
        var submission = await Send(new GetFinalSubmissionByIdRequest(submissionId));
        if (submission == null || submission.Enrollment.MentorId != mentor.Id) return NotFound();

        var data = await GetMentorStudentDataAsync(mentor.Id, submission.Enrollment.StudentId);
        var view = new MentorCorrectedSubmissionView(submission, data, _fileStorageService, _avatarService);
        return Json(view);
    }

    [HttpGet("/mentor/submissions/to-review")]
    public async Task<ActionResult<List<ReviewSummaryView>>> GetOpenSubmissions()
    {
        var mentor = await GetCurrentUser();
        if (mentor == null || mentor.IsMentor == false) return BadRequest();
        var submissions = await Send(new GetSubmissionsForReviewRequest(mentor.Id));
        var activity = await Send(new GetStudentActivityRequest(mentor.Id));
        var view = submissions.Select(s => new ReviewSummaryView(s, activity.Contains(s.Enrollment.StudentId), _avatarService)).ToList();
        return Json(view);
    }

    [HttpGet("/mentor/submissions/review/{submissionId:int}")]
    public async Task<ActionResult<ReviewDetailView>> GetOpenSubmission(int submissionId)
    {
        // First get the mentor and the submission selected
        var mentor = await GetCurrentUser();
        if (mentor == null || mentor.IsMentor == false) return BadRequest();

        // Please note, including the mentor id is an additional check
        var exists = await Send(new IsSubmissionReviewableRequest(mentor.Id, submissionId));
        if (exists == false) return NotFound();

        var submission = await Send(new GetSubmissionForReviewByIdRequest(mentor.Id, submissionId));
        if (submission == null) return Json(new { AlreadyReviewed = true });

        var data = await GetMentorStudentDataAsync(mentor.Id, submission.Enrollment.StudentId);
        var answers = GetAnswersForSubmissionView(submission);
        var view = new ReviewDetailView(submission, answers, data, _fileStorageService, _avatarService);

        return Json(view);
    }

    [HttpPost("/mentor/submissions/review")]
    public async Task<ActionResult> ReviewSubmissionForMentor([FromBody] SubmissionReviewPostModel review)
    {
        // Get the mentor that has preformed the review
        var mentor = await GetCurrentUser();

        // Get the result and return
        if (mentor == null || mentor.IsMentor == false) return BadRequest();
        
        // Update the user took some action
        await Send(new UpdateUserLastActivityRequest(mentor.Id));
        
        var saved = await ReviewSubmission(review, mentor);
        return saved ? Ok() : BadRequest();
    }

    internal static List<CorrectedAnswerView> GetAnswersForSubmissionView(LessonSubmission submission)
    {
        // Match the answers with the question
        var answers = new List<CorrectedAnswerView>();
        if (submission.Imported || submission.Answers == null) return null;

        foreach (var answer in submission.Answers)
        {
            var question = submission.Lesson.Content.First(c => c.Type == answer.QuestionType && c.Id == answer.QuestionId);
            switch (answer.QuestionType)
            {
                case ContentBlockTypes.QuestionBoolean:
                    answers.Add(new CorrectedAnswerView(answer, question as BoolQuestion));
                    break;
                case ContentBlockTypes.QuestionMultipleChoice:
                    answers.Add(new CorrectedAnswerView(answer, question as MultipleChoiceQuestion));
                    break;
                case ContentBlockTypes.QuestionOpen:
                    answers.Add(new CorrectedAnswerView(answer, question as OpenQuestion));
                    break;
            }
        }

        return answers;
    }

    private async Task<MentorStudentData> GetMentorStudentDataAsync(int mentorId, int studentId)
    {
        // Load existing data or create a new object
        return await Send(new GetStudentRemarksRequest(mentorId, studentId))
            ?? new MentorStudentData { MentorId = mentorId, StudentId = studentId };
    }
}