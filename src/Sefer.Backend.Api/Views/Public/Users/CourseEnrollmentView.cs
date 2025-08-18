using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Public.Users;

/// <summary>
/// Class used to return the result if a student can roll on to a course
/// </summary>
/// <inheritdoc />
/// <remarks>
/// Creates a new View
/// </remarks>
/// <param name="course">The model of the view</param>
/// <param name="canEnroll">True when the student can enroll else false</param>
/// <param name="hasIssue">True when the student has a personal mentor that is not teaching the course else false</param>
/// <inheritdoc />
public sealed class CourseEnrollmentView(Course course, bool canEnroll, bool hasIssue) : CourseView(course)
{
    /// <summary>
    /// Holds true when the user can enroll else false
    /// </summary>
    public readonly bool CanEnroll = canEnroll;

    /// <summary>
    /// Holds if when the student has a personal mentor, the mentor is not teaching
    /// the course the user wants to enroll to
    /// </summary>
    public readonly bool HasPersonalMentorIssue = hasIssue;
}
