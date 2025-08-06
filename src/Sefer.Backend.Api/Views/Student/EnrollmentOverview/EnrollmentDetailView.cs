// This is a view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Views.Shared.Enrollments;

namespace Sefer.Backend.Api.Views.Student.EnrollmentOverview;

/// <summary>
/// This view contains detailed information of the enrollment of a user in a course.
/// It includes the submitted lessons and lessons information
/// </summary>
public class EnrollmentDetailView : EnrollmentView
{
    /// <summary>
    /// A list with submission with in the enrollment
    /// </summary>
    /// <remarks>Use a list of objects to deal with serialization issues of .net</remarks>
    public List<object> Submissions { get; set; }

    /// <summary>
    /// The grade of the user for the course
    /// </summary>
    public readonly double? Grade;

    /// <summary>
    /// Holds if this course has a grade
    /// </summary>
    public bool HasGrade => Grade.HasValue;

    public EnrollmentDetailView(Enrollment enrollment, IFileStorageService fileStorageService) : base(enrollment, fileStorageService)
    {
        double? grade = null;
        if (enrollment.Grade.HasValue) grade = Math.Round(enrollment.Grade.Value * 10, 1);
        if (enrollment.IsCourseCompleted && enrollment.LessonSubmissions.Any())
        {
            var grades = enrollment.LessonSubmissions
                .Where(e => e.Grade.HasValue).Select(e => e.Grade.Value).ToList();
            if (grades.Count == enrollment.LessonSubmissions.Count) grade = Math.Round(grades.Average() * 10, 1);
        }
        Grade = grade;

        Submissions = Model.LessonSubmissions
            .Where(l => l.IsFinal)
            .OrderByDescending(s => s.SubmissionDate)
            .Select(submission => CreateView(submission, submission.Lesson, fileStorageService))
            .Cast<object>()
            .ToList();
    }

    /// <summary>
    /// Creates a view for the submission. This function will determine if a corrected or uncorrected view should be generated
    /// </summary>
    private ISubmissionResultView CreateView(LessonSubmission submission, Lesson lesson, IFileStorageService fileStorageService)
    {
        if (!Model.IsSelfStudy && !submission.ResultsStudentVisible) return new UncorrectedSubmissionResultView(submission, Model, fileStorageService);

        var answers = new List<CorrectedAnswerView>();
        if (!submission.Imported && submission.Answers != null)
        {
            foreach (var answer in submission.Answers)
            {
                var answerView = GetAnswerView(answer, submission);
                if (answerView != null) answers.Add(answerView);
            }
        }
        var view = new CorrectedSubmissionResultView(submission, lesson, answers, Model, fileStorageService);
        return view;
    }

    /// <summary>
    /// Converts a question answer to a correct answer view
    /// </summary>
    /// <returns>A view when the question is found else null</returns>
    private static CorrectedAnswerView GetAnswerView(QuestionAnswer answer, LessonSubmission submission)
    {
        var question = submission.Lesson.Content.FirstOrDefault(c => c.Id == answer.QuestionId && c.Type == answer.QuestionType);
        if (question == null) return null;

        return answer.QuestionType switch
        {
            ContentBlockTypes.QuestionBoolean => new CorrectedAnswerView(answer, question as BoolQuestion),
            ContentBlockTypes.QuestionOpen => new CorrectedAnswerView(answer, question as OpenQuestion),
            ContentBlockTypes.QuestionMultipleChoice => new CorrectedAnswerView(answer, question as MultipleChoiceQuestion),
            _ => null,
        };
    }
}
