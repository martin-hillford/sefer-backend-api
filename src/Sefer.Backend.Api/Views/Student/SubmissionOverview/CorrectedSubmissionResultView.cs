// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Views.Shared.Courses;
using Sefer.Backend.Api.Views.Shared.Enrollments;
using Sefer.Backend.Api.Views.Student.EnrollmentOverview;

namespace Sefer.Backend.Api.Views.Student.SubmissionOverview;

/// <summary>
/// A view on a submission that is corrected by the mentor
/// </summary>
public class CorrectedSubmissionResultView : BaseSubmissionResultView, ISubmissionResultView
{
    /// <summary>
    /// The total number of answers given
    /// </summary>
    public int TotalAnswersGiven => CorrectedAnswers.Count(a => a.QuestionType != ContentBlockTypes.QuestionOpen);

    /// <summary>
    /// Gets the number of total correct answer
    /// </summary>
    /// <returns></returns>
    public int CorrectAnswersGiven => CorrectedAnswers.Count(a => a.QuestionType != ContentBlockTypes.QuestionOpen && a.IsValid == true);

    /// <summary>
    /// Return the number of wrong answer given
    /// </summary>
    public int WrongAnswersGiven => TotalAnswersGiven - CorrectAnswersGiven;

    /// <summary>
    /// Holds (when the submission was final and the course is a self study course) the grade for this submission.null. It represents a value between 0 and 1
    /// </summary>
    /// <returns></returns>
    public readonly float? Grade;

    /// <summary>
    /// Holds if this course has a grade
    /// </summary>
    public bool HasGrade => Grade.HasValue;

    /// <summary>
    /// Gets is corrected answers are included
    /// </summary>
    public bool CorrectAnswersIncluded => true;

    /// <summary>
    /// A view on the lesson that the submission belongs to
    /// </summary>
    public LessonView Lesson { get; }

    /// <summary>
    /// The date the (final) submission was done
    /// </summary>
    public DateTime SubmissionDate { get; }

    /// <summary>
    /// Holds the review date for this submission
    /// </summary>
    public readonly DateTime? ReviewDate;

    /// <summary>
    /// Holds if this submission is review by a mentor
    /// </summary>
    public readonly bool ReviewedByMentor;

    /// <summary>
    /// A list of all the correct answers of the student
    /// </summary>
    public readonly IReadOnlyCollection<CorrectedAnswerView> CorrectedAnswers;

    /// <summary>
    /// The name of the course that this submission is about
    /// </summary>
    public string CourseName { get; private set; }

    /// <summary>
    /// The name of the student that posted this submissions
    /// </summary>
    public string StudentName { get; private set; }

    /// <summary>
    /// The name of the mentor that is overseeing this submissions
    /// </summary>
    public string MentorName { get; private set; }

    /// <summary>
    /// The id of the mentor that is overseeing this submissions
    /// </summary>
    public int? MentorId { get; private set; }

    /// <summary>
    /// The id of the course that this submission is about
    /// </summary>
    public readonly int CourseId;

    /// <summary>
    /// Full view of the course
    /// </summary>
    public readonly CourseDisplayView Course;

    /// <summary>
    /// Creates a new view, this view will include a list of answers
    /// </summary>
    /// <inheritdoc />
    public CorrectedSubmissionResultView(LessonSubmission submission, IFileStorageService fileStorageService) : base(submission.Enrollment.IsSelfStudy, submission.Enrollment.Id)
    {
        if (submission.SubmissionDate.HasValue) SubmissionDate = submission.SubmissionDate.Value;
        Lesson = new LessonView(submission.Lesson, submission.Enrollment, fileStorageService);
        SubmissionId = submission.Id;
        if (submission.Grade.HasValue) Grade = (float)Math.Round(submission.Grade.Value * 10, 1);

        var answers = new List<CorrectedAnswerView>();
        if (submission.Imported == false && submission.Answers != null)
        {
            foreach (var answer in submission.Answers)
            {
                if (submission.Lesson.Content == null || submission.Lesson.Content.Any() == false) continue;
                var question = submission.Lesson.Content.Single(c => c.Type == answer.QuestionType && c.Id == answer.QuestionId);
                switch (question.Type)
                {
                    case ContentBlockTypes.QuestionOpen:
                        answers.Add(new CorrectedAnswerView(answer, question as OpenQuestion));
                        break;
                    case ContentBlockTypes.QuestionBoolean:
                        answers.Add(new CorrectedAnswerView(answer, question as BoolQuestion));
                        break;
                    case ContentBlockTypes.QuestionMultipleChoice:
                        answers.Add(new CorrectedAnswerView(answer, question as MultipleChoiceQuestion));
                        break;
                }
            }
        }
        CorrectedAnswers = answers.OrderBy(a => a.SequenceId).ToList().AsReadOnly();
        CourseId = submission.Enrollment.CourseRevision.CourseId;
        StudentName = submission.Enrollment.Student.Name;
        MentorName = submission.Enrollment.IsSelfStudy ? string.Empty : submission.Enrollment.Mentor.Name;
        MentorId = submission.Enrollment.MentorId;
        CourseName = submission.Enrollment.CourseRevision.Course.Name;
        ReviewDate = submission.ReviewDate;
        ReviewedByMentor = submission.ReviewDate != null;
        ResultsStudentVisible = submission.ResultsStudentVisible;
        Course = new CourseDisplayView(submission.Enrollment.CourseRevision.Course, fileStorageService);
    }
}