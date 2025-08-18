namespace Sefer.Backend.Api.Data.Handlers.Students;

public class IsStudentEnrollableForCourseHandler(IServiceProvider serviceProvider)
    : Handler<IsStudentEnrollableForCourseRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsStudentEnrollableForCourseRequest request, CancellationToken token)
    {
        // A mentor cannot enroll for a course
        var student = await Send(new GetUserByIdRequest(request.StudentId), token);
        if (student == null || student.IsMentor) return false;

        // Check if the course is actually published
        var courseRevision = await Send(new GetPublishedCourseRevisionRequest(request.CourseId), token);
        if (courseRevision == null) return false;

        // First, check if the student already has an active course
        var activeEnrollment = await Send(new GetActiveEnrollmentsOfStudentRequest(request.StudentId), token);
        if (activeEnrollment != null) return false;

        // check if the student has a retake for this course
        var hasRetake = await Send(new HasStudentRetakeForCourseRequest(request.StudentId, request.CourseId), token);
        if (hasRetake) return true;

        // Check if the user has already taken the course
        var hasTaken = await Send(new HasStudentCompletedCourseRequest(request.StudentId, request.CourseId), token);
        return hasTaken == false;

    }
}