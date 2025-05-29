// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Views.Student.SubmissionOverview;

/// <summary>
/// A view on a submission that is not corrected by the mentor
/// </summary>
/// <inheritdoc />
public class UncorrectedSubmissionResultView : EnrollmentOverview.UncorrectedSubmissionResultView
{
    #region Properties

    /// <summary>
    /// Include a list of answers given by the student
    /// </summary>
    public readonly IReadOnlyCollection<Shared.Enrollments.AnswerView> Answers;

    /// <summary>
    /// The id of the course that this submission is about
    /// </summary>
    public readonly int CourseId;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a view on submission that is not (yet) correct by a mentor
    /// </summary>
    public UncorrectedSubmissionResultView(LessonSubmission submission, Enrollment enrollment, IFileStorageService fileStorageService) : base(submission, enrollment, fileStorageService)
    {
        var answers = new List<Shared.Enrollments.AnswerView>();
        if (submission.Imported == false && submission.Answers != null)
        {
            foreach (var answer in submission.Answers)
            {
                var question = submission.Lesson.Content.Single(c => c.Type == answer.QuestionType && c.Id == answer.QuestionId);
                switch (question.Type)
                {
                    case ContentBlockTypes.QuestionOpen:
                        answers.Add(new Shared.Enrollments.AnswerView(answer, question as OpenQuestion));
                        break;
                    case ContentBlockTypes.QuestionBoolean:
                        answers.Add(new Shared.Enrollments.AnswerView(answer, question as BoolQuestion));
                        break;
                    case ContentBlockTypes.QuestionMultipleChoice:
                        answers.Add(new Shared.Enrollments.AnswerView(answer, question as MultipleChoiceQuestion));
                        break;
                }
            }
        }
        Answers = answers.AsReadOnly();
        CourseName = enrollment.CourseRevision.Course.Name;
        CourseId = enrollment.CourseRevision.CourseId;
    }

    #endregion
}