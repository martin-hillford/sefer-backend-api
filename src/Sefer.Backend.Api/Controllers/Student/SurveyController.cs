using Sefer.Backend.Api.Data.Requests.Surveys;
using Sefer.Backend.Api.Models.Student.Profile;

namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class SurveyController(IServiceProvider serviceProvider) : GrantController(serviceProvider)
{
    [HttpPost("/student/lessons/survey")]
    public async Task<ActionResult> SubmitSurvey([FromBody] SurveyResultPostModel result)
    {
        // try to load the student that is updating its profile (404)
        if (result == null || ModelState.IsValid == false) return BadRequest();

        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        // Now load the enrollment that is provided by the result and check if it's a valid submit
        var enrollment = await Send(new GetEnrollmentByIdRequest(result.EnrollmentId));
        if (enrollment == null || enrollment.StudentId != student.Id) return BadRequest();
        if (enrollment.SurveySubmitted || enrollment.IsCourseCompleted == false) return BadRequest();

        // Now load the course revision and survey based on the enrollment
        var courseRevision = await Send(new GetCourseRevisionByIdRequest(enrollment.CourseRevisionId));
        if (courseRevision == null) return StatusCode(500);
        var survey = await Send(new GetSurveyByCourseRevisionRequest(courseRevision.Id));
        if (survey == null) return StatusCode(500);

        // Now based on the contents of the survey process his result
        if (survey.EnableSurvey == false) return BadRequest();
        var surveyResult = new SurveyResult
        {
            CreationDate = DateTime.UtcNow,
            SocialPermissions = result.SocialPermissions && survey.EnableSocialPermissions,
            EnrollmentId = enrollment.Id
        };

        // Deal with the course rating
        if (survey.EnableCourseRating)
        {
            var courseRating = new CourseRating
            { CourseId = courseRevision.CourseId, Rating = (byte)(result.CourseRating * 2) };
            var courseRatingAdded = await Send(new AddCourseRatingRequest(courseRating));
            if (courseRatingAdded == false) return StatusCode(500);
            surveyResult.CourseRatingId = courseRating.Id;
        }

        // Deal with the mentor rating
        if (survey.EnableMentorRating && enrollment.MentorId.HasValue && courseRevision.AllowSelfStudy == false)
        {
            var mentorRating = new MentorRating
            { MentorId = enrollment.MentorId.Value, Rating = (byte)(result.MentorRating * 2) };
            var mentorRatingAdded = await Send(new AddMentorRatingRequest(mentorRating));
            if (mentorRatingAdded == false) return StatusCode(500);
            surveyResult.MentorRatingId = mentorRating.Id;
        }

        // Set the social permissions
        if (survey.EnableTestimonial) surveyResult.Text = result.Text;

        // Ratings are added, everything is set, insert the result and update the enrollment
        enrollment.SurveySubmitted = true;
        await Send(new UpdateSingleEnrollmentPropertyRequest(enrollment, nameof(Enrollment.SurveySubmitted)));
        var inserted = await Send(new AddSurveyResultRequest(surveyResult));

        // Insert the result and return to the student
        return inserted ? StatusCode(200) : StatusCode(500);
    }
}