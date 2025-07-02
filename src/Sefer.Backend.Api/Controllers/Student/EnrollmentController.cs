using Sefer.Backend.Api.Data.Algorithms;
using Sefer.Backend.Api.Data.Models.Courses.Rewards;
using Sefer.Backend.Api.Data.Requests.Curricula;
using Sefer.Backend.Api.Data.Requests.Rewards;
using Sefer.Backend.Api.Views.Shared.Courses;
using Sefer.Backend.Api.Views.Student;
using Sefer.Backend.Api.Views.Student.EnrollmentOverview;

namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class EnrollmentController(IServiceProvider serviceProvider) : UserController(serviceProvider)
{
    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();

    private readonly ICryptographyService _cryptographyService = serviceProvider.GetService<ICryptographyService>();

    [HttpGet("/student/lessons/current")]
    [ProducesResponseType(typeof(CurrentLessonView), 200)]
    public async Task<ActionResult> CurrentLesson()
    {
        // try to load the student that is updating his profile (404)
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return NotFound();

        // gets the current lesson
        // To prevent from jumping ahead, the current lesson is always delivered via one api url and not per id.
        var (lesson, lessonSubmission, enrollment) = await Send(new GetCurrentLessonRequest(student.Id));
        if (lesson == null) return NotFound();

        // And send the result back to the user
        var view = new CurrentLessonView(lesson, enrollment, lessonSubmission, _fileStorageService);
        return Json(view);
    }

    [HttpGet("/student/lessons/suggestion")]
    [ProducesResponseType(typeof(CourseDisplayView), 200)]
    public async Task<ActionResult> GetCourseSuggestion()
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        var recommender = new CourseRecommender(ServiceProvider);
        var course = await recommender.GetRecommendation(student.Id, true);
        if (course == null) return NotFound();

        var view = new CourseDisplayView(course, _fileStorageService);
        return Json(view);
    }

    [HttpGet("/student/curricula-progress")]
    [ProducesResponseType(typeof(CurriculumProgressCollectionView), 200)]
    public async Task<ActionResult> GetCurriculumProgress()
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        var calculator = new CurriculumProgressCalculator(ServiceProvider);
        var progress = await calculator.GetCurriculumProgress(student.Id);

        var receivedGrants = await Send(new GetReceivedGrantsRequest(student.Id));
        var curriculumGrants = receivedGrants.Where(g => g.Reward.Type == RewardTypes.Curriculum);

        var curricula = await Send(new GetCurriculaRequest());
        var grants = new List<CurriculumGrantView>();

        foreach (var grant in curriculumGrants)
        {
            var curriculum = curricula.SingleOrDefault(c => c.Id == grant.TargetReached);
            if (curriculum == null) continue;
            grants.Add(new CurriculumGrantView(grant, curriculum));
        }

        var dictionary = curricula.ToDictionary(c => c.Id, c => c);
        var progressView = progress
            .Where(p => dictionary.ContainsKey(p.Key))
            .Select(p => new CurriculumProgressView(dictionary[p.Key], p.Value))
            .OrderBy(p => p.Level)
            .ToList();

        var view = new CurriculumProgressCollectionView { Curricula = progressView, Grants = grants };
        return Json(view);
    }

    [HttpGet("/student/taken-courses")]
    public async Task<ActionResult<List<TakenCourseView>>> GetTakenCourses()
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        var enrollments = await Send(new GetTakenCoursesOfStudentRequest(student.Id));
        var studentCounts = await Send(new GetStudentsCountPerCourseRequest());
        var ratings = await Send(new GeRatingsForCoursesRequest());
        var readingTime = await Send(new GetCourseReadingTimeRequest());

        var view = new List<TakenCourseView>();
        foreach (var enrollment in enrollments)
        {
            var courseId = enrollment.CourseRevision.CourseId;
            var rating = ratings.TryGetValue(courseId, out var rating1) ? rating1 : ((byte)0, 0);
            var studentCount = studentCounts.GetValueOrDefault(courseId, 0);
            view.Add(new TakenCourseView(enrollment, readingTime[courseId], rating, _fileStorageService, _cryptographyService, studentCount));
        }

        // And return
        return Json(view);
    }

    [HttpGet("/student/enrollments/{enrollmentId:int}")]
    public async Task<ActionResult<EnrollmentDetailView>> GetEnrollment(int enrollmentId)
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        // Load the enrollment
        var enrollment = await Send(new GetEnrollmentByIdExtensivelyRequest(enrollmentId));

        if (enrollment == null) return NotFound();
        if (enrollment.StudentId != student.Id) return Forbid();

        // Load the details on the submission
        foreach (var submission in enrollment.LessonSubmissions)
        {
            submission.Lesson.Content = await Send(new GetLessonContentRequest(submission.Lesson));

            // Check if the answers are set
            foreach (var answer in submission.Answers)
            {
                var question = submission.Lesson.Content.First(c => c.Type == answer.QuestionType && c.Id == answer.QuestionId);
                answer.IsCorrectAnswer = answer.IsCorrect(question);
            }
        }

        var view = new EnrollmentDetailView(enrollment, _fileStorageService);
        return Json(view);
    }

    [HttpPost("/student/enrollments/unroll")]
    public async Task<ActionResult> UnrollStudent([FromBody] int enrollmentId)
    {
        // Get the enrollment to unroll
        var enrollment = await Send(new GetEnrollmentByIdRequest(enrollmentId));
        if (enrollment == null) return NotFound();
        if (enrollment.StudentId != UserId) return Forbid();

        await Send(new UpdateUserLastActivityRequest(enrollment.StudentId));
        var success = await Send(new UnEnrollRequest(enrollment.Id));
        return success ? NoContent() : StatusCode(500);
    }
}