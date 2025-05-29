// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// A view to show a student which course he has taken
/// </summary>
public class TakenCourseView : CourseWithAttributesView
{
    /// <summary>
    /// The grade of the user for the course
    /// </summary>
    public readonly double? Grade;

    /// <summary>
    /// Holds if this course has a grade
    /// </summary>
    public bool HasGrade => Grade.HasValue;

    /// <summary>
    /// The date the enrollment is closed.
    /// Either because the user ended his enrollment or did complete the course
    /// </summary>
    public readonly DateTime? ClosureDate;

    /// <summary>
    /// The date the object was created
    /// </summary>
    public readonly DateTime CreationDate;

    /// <summary>
    /// Holds if the student has completed the course. Can be set because the user submitted all the lessons of course to mentor.
    /// Or can be set by an administrator because he knows of completion in the past.
    /// </summary>
    public readonly bool IsCourseCompleted;

    /// <summary>
    /// Gets if the enrollment is active
    /// </summary>
    public readonly bool IsActive;

    /// <summary>
    /// The id of the enrollment of the student
    /// </summary>
    public readonly int EnrollmentId;

    /// <summary>
    /// The url to the diploma
    /// </summary>
    public readonly string DiplomaUrl;

    /// <summary>
    /// Holds if the user will have a diploma for this course
    /// </summary>
    public readonly bool HasDiploma;

    /// <summary>
    /// Creates a new View
    /// </summary>
    public TakenCourseView(Enrollment enrollment, CourseReadingTime readingTime, (byte Rating, int Count) ratings, IFileStorageService fileStorageService, ICryptographyService cryptographyService, int studentCount)
        : base(enrollment.CourseRevision, readingTime, ratings, fileStorageService, studentCount)
    {
        Grade = GetGrade(enrollment);
        ClosureDate = enrollment.ClosureDate;
        IsCourseCompleted = enrollment.IsCourseCompleted;
        IsActive = enrollment.IsActive;
        CreationDate = enrollment.CreationDate;
        EnrollmentId = enrollment.Id;

        HasDiploma = IsCourseCompleted && Grade is >= 7;
        if (HasDiploma)
        {
            var hash = cryptographyService.Hash(EnrollmentId.ToString());
            hash = BitConverter.ToString(Convert.FromBase64String(hash)).Replace("-", string.Empty).ToLower();
            DiplomaUrl = $"/student/enrollments/{EnrollmentId}/diploma/{hash}";
        }
    }

    private static double? GetGrade(Enrollment enrollment)
    {
        if (enrollment.IsCourseCompleted && enrollment.LessonSubmissions.Any())
        {
            var grades = enrollment.LessonSubmissions.Where(e => e.Grade.HasValue).Select(e => e.Grade.Value).ToList();
            if (grades.Count == enrollment.LessonSubmissions.Count) return Math.Round(grades.Average() * 10, 1);
        }
        return null;
    }
}