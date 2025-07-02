using Sefer.Backend.Api.Data.Requests.Surveys;
using Sefer.Backend.Api.Data.Validation;
using Sefer.Backend.Api.Models.Student.Profile;
using Sefer.Backend.Api.Views.Shared.Courses.Surveys;
using Sefer.Backend.Api.Views.Shared.Enrollments;
using Sefer.Backend.Api.Views.Student;
using EnrollmentOverview_CorrectedSubmissionResultView = Sefer.Backend.Api.Views.Student.EnrollmentOverview.CorrectedSubmissionResultView;
using QuestionAnswer = Sefer.Backend.Api.Data.Models.Enrollments.QuestionAnswer;

namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class SubmitLessonController(IServiceProvider serviceProvider) : GrantController(serviceProvider)
{
    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();

    private readonly INotificationService _notificationService = serviceProvider.GetService<INotificationService>();

    [HttpPost("/student/lessons/submit")]
    [ProducesResponseType(typeof(BaseSubmissionResultView), 201)]
    [ProducesResponseType(typeof(EnrollmentOverview_CorrectedSubmissionResultView), 202)]
    public async Task<ActionResult> SubmitLesson([FromBody] SubmissionPostModel submission)
    {
        // try to load the student that is updating his profile (403)
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        // get the current enrollment of the student
        var active = await Send(new GetActiveEnrollmentOfStudentRequest(student.Id));
        if (active == null) return NotFound();
        if (active.OnPaper) return BadRequest();
        
        // Update the user took some action
        await Send(new UpdateUserLastActivityRequest(student.Id));

        // gets the current lesson
        var (lesson, lessonSubmission, enrollment) = await Send(new GetCurrentLessonRequest(student.Id));
        if (lesson == null) return NotFound();

        // check if the submission is valid
        if (IsValidSubmission(lesson, submission) == false) return BadRequest("Invalid submission");

        // There are (currently) three options: not-final, final & self-study  and final & no-self-study
        // For readability those three options have separate function

        // 1) not-final post: the answers of the users should be saved
        if (submission.Final == false)
            return await SubmitLessonNotFinal(submission, enrollment, lesson, lessonSubmission);

        // 2) self-study-post: the answers should be graded and send to the user
        if (enrollment.IsSelfStudy) return await SubmitSelfStudy(submission, enrollment, lesson, lessonSubmission);

        // 3) If those two are not the case, this is a mentor study submission
        return await SubmitMentorStudy(student, submission, enrollment, lesson, lessonSubmission);
    }

    private async Task<ActionResult> SubmitLessonNotFinal(SubmissionPostModel posted, Enrollment enrollment, Lesson lesson, LessonSubmission current)
    {
        // Convert to a database models
        if (current is { Imported: true }) return BadRequest();
        var (submission, answers) = await Convert(posted, enrollment, current, lesson);

        // Save the result
        var saved = await Send(new SaveSubmissionRequest(submission, answers));
        if (saved == false) return StatusCode(500);

        // Return the result
        var result = new BaseSubmissionResultView(enrollment.IsSelfStudy, enrollment.Id) { SubmissionId = submission.Id };
        return Json(result, 201);
    }

    private async Task<ActionResult> SubmitSelfStudy(SubmissionPostModel postedSubmission, Enrollment enrollment, Lesson lesson, LessonSubmission currentSubmission)
    {
        // Convert to a database models
        if (currentSubmission is { Imported: true }) return BadRequest();
        var (submission, answers) = await Convert(postedSubmission, enrollment, currentSubmission, lesson);

        // Save the result
        var saved = await Send(new SaveSubmissionRequest(submission, answers));
        if (saved == false) return StatusCode(500);

        var courseRevision = await Send(new GetCourseRevisionByIdRequest(enrollment.CourseRevisionId));
        if (courseRevision == null) return BadRequest();

        // Create the corrected answers for the view
        var correctedAnswers = new List<CorrectedAnswerView>();
        foreach (var answer in answers)
        {
            switch (answer.QuestionType)
            {
                case ContentBlockTypes.QuestionBoolean:
                    var boolQuestion = await Send(new GetBoolQuestionByIdRequest(answer.QuestionId));
                    correctedAnswers.Add(new CorrectedAnswerView(answer, boolQuestion));
                    break;
                case ContentBlockTypes.QuestionOpen:
                    var openQuestion = await Send(new GetOpenQuestionByIdRequest(answer.QuestionId));
                    correctedAnswers.Add(new CorrectedAnswerView(answer, openQuestion));
                    break;
                case ContentBlockTypes.QuestionMultipleChoice:
                    var multipleChoiceQuestion =
                        await Send(new GetMultipleChoiceQuestionByIdRequest(answer.QuestionId));
                    correctedAnswers.Add(new CorrectedAnswerView(answer, multipleChoiceQuestion));
                    break;
            }
        }

        // Create the view
        var view = new EnrollmentOverview_CorrectedSubmissionResultView(submission, correctedAnswers, enrollment, _fileStorageService);

        // check if this is final submission for the enrollment
        var finished = await CheckIsEnrollmentIsFinished(enrollment);

        // Situation 1) The enrollment is not completed a next lesson will be available - 201 should be returned
        if (finished == false) return Json(view, 201);

        // Situation 2) If the enrollment is finished, the survey must be loaded and returned
        var survey = await Send(new GetSurveyByCourseRevisionRequest(courseRevision.Id));
        view.Survey = new SurveyView(survey);

        // Additionally, the rewards must be calculated
        view.Grants = await HandleGrants(enrollment);
        return Json(view, 202);
    }

    private async Task<ActionResult> SubmitMentorStudy(User student, SubmissionPostModel postedSubmission, Enrollment enrollment, Lesson lesson, LessonSubmission currentSubmission)
    {
        // Check if the student is allowed to submit
        if (currentSubmission is { Imported: true }) return BadRequest();
        var courseRevision = await Send(new GetCourseRevisionByIdRequest(enrollment.CourseRevisionId));
        if (courseRevision == null) return BadRequest();

        var course = await Send(new GetCourseByIdRequest(courseRevision.CourseId));
        if (course == null) return BadRequest();
        var isAllowed = await IsStudentAllowedToSubmit(student, course);
        if (isAllowed == false) return BadRequest(new { SubmissionLimitReached = true });

        // Convert to a database models
        var (submission, answers) = await Convert(postedSubmission, enrollment, currentSubmission, lesson);

        // Save the result
        var saved = await Send(new SaveSubmissionRequest(submission, answers));
        if (saved == false) return StatusCode(500);

        // Determine the correctness for all the answers
        var result = new MentorSubmissionResultView(enrollment.Id) { SubmissionId = submission.Id, };

        // check if this is final submission for the enrollment
        var finished = await CheckIsEnrollmentIsFinished(enrollment);

        // Notify the mentor and check if the course is finished
        if (postedSubmission.Final && enrollment.MentorId.HasValue)
        {
            var mentor = await Send(new GetUserByIdRequest(enrollment.MentorId.Value));
            var notified = await _notificationService.SendLessonSubmittedNotificationAsync(submission.Id, mentor, student);
            if (!notified) return StatusCode(500);
        }

        // If the enrollment is not finished (a next lesson is available) 201 should be returned
        if (!finished) return Json(result, 201);

        // If the enrollment is finished, the survey must be loaded and returned
        var survey = await Send(new GetSurveyByCourseRevisionRequest(courseRevision.Id));
        result.Survey = new SurveyView(survey);

        // Additionally, the rewards must be calculated
        result.Grants = await HandleGrants(enrollment);

        return Json(result, 202);
    }

    private static bool IsValidSubmission(Lesson lesson, SubmissionPostModel submission)
    {
        if (submission == null) return false;
        if (submission.Final == false) return true;
        if (submission.Answers == null) return false;
        var questions = lesson.Content.Where(c => c.IsQuestion).ToList();

        // The number of answers should match the number of questions
        if (submission.Answers.Count != questions.Count) return false;

        // Every question should have answer
        var answers = submission.Answers.ToLookup(a => a.QuestionId);
        foreach (var question in questions)
        {
            var contains = answers.Contains(question.Id) &&
                           answers[question.Id].Any(a => a.QuestionType == question.Type);
            if (contains == false) return false;
            var answer = answers[question.Id].FirstOrDefault(a => a.QuestionType == question.Type);
            if (answer == null) return false;

            switch (question.Type)
            {
                case ContentBlockTypes.QuestionOpen:
                    if (string.IsNullOrEmpty(answer.Answer)) return false;
                    break;
                case ContentBlockTypes.QuestionBoolean:
                    if (BooleanStringConvertor.Convert(answer.Answer) == null) return false;
                    break;
                case ContentBlockTypes.QuestionMultipleChoice:
                    var multipleChoiceQuestion = (MultipleChoiceQuestion)question;
                    // here is the thing, answers for multiple choice question are provided as a comma separated list of id
                    var choiceIds = answer.Answer.Split(',');
                    var hasChoice = choiceIds.All(ac => multipleChoiceQuestion.Choices.Any(c => c.Id.ToString() == ac));
                    if (hasChoice == false) return false;
                    break;
                default:
                    return false;
            }
        }

        return true;
    }

    private async Task<(LessonSubmission Submission, List<QuestionAnswer> Answers)> Convert(SubmissionPostModel submission, Enrollment enrollment, LessonSubmission existing, Lesson lesson)
    {
        // deal with the submission first
        var answers = new List<QuestionAnswer>();
        if (existing == null) existing = submission.CreateNew(enrollment, lesson);
        else existing.IsFinal = submission.Final;

        // When final set the submission date
        if (existing.IsFinal) existing.SubmissionDate = DateTime.UtcNow;

        // Also check the content that none existing question are not added
        var content = new Dictionary<ContentBlockTypes, HashSet<int>>();
        foreach (var block in lesson.Content)
        {
            if (content.ContainsKey(block.Type) == false) content.Add(block.Type, new HashSet<int>());
            content[block.Type].Add(block.Id);
        }

        // deal with the questions
        var dbAnswers = (await Send(new GetSubmissionAnswersRequest(existing.Id))).ToLookup(a => a.QuestionId);
        if (submission.Answers == null) return (existing, answers);
        foreach (var answer in submission.Answers)
        {
            // Check for non-existing answers
            if (content.TryGetValue(answer.QuestionType, out var value) == false) continue;
            if (value.Contains(answer.QuestionId) == false) continue;

            // determine if a similar question can be found
            var contains = dbAnswers.Contains(answer.QuestionId) &&
                           dbAnswers[answer.QuestionId].Any(a => a.QuestionType == answer.QuestionType);
            if (contains == false) answers.Add(answer.Convert());
            else
            {
                var dbAnswer = dbAnswers[answer.QuestionId].First(a => a.QuestionType == answer.QuestionType);
                dbAnswer.TextAnswer = answer.Answer;
                answers.Add(dbAnswer);
            }
        }

        // return the result
        return (existing, answers);
    }

    private async Task<bool> CheckIsEnrollmentIsFinished(Enrollment enrollment)
    {
        var submissionCount = await Send(new GetFinalSubmittedLessonsCountRequest(enrollment.Id));
        var courseRevision = await Send(new GetCourseRevisionByIdRequest(enrollment.CourseRevisionId));
        if (courseRevision == null) return false;

        var lessonCount = await Send(new GetLessonsCountOfCourseRevisionRequest(courseRevision.Id));
        var valid = submissionCount == lessonCount;
        if (!valid) return false;

        return await Send(new CompleteEnrollmentRequest(enrollment.Id));
    }

    private async Task<bool> IsStudentAllowedToSubmit(User student, Course course)
    {
        var controller = new SubmissionController(ServiceProvider);
        return await controller.IsStudentAllowedToSubmit(student, course);
    }
}