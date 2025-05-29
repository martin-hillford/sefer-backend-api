// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// A view on a submission that is not (yet) corrected by the mentor
/// </summary>
/// <inheritdoc />
public class SubmissionResultView : BaseSubmissionResultView
{
    #region Properties

    /// <summary>
    /// The answers that the student has given
    /// </summary>
    /// <returns></returns>
    public readonly IReadOnlyCollection<Shared.Enrollments.AnswerView> Answers;

    /// <summary>
    /// Gets is corrected answers are included
    /// </summary>
    public bool Corrected => false;

    /// <summary>
    /// A view on the lesson that the submission belongs to
    /// </summary>
    public readonly LessonView Lesson;

    /// <summary>
    /// The date the (final) submission was done
    /// </summary>
    public readonly DateTime SubmissionDate;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new SubmissionResultView
    /// </summary>
    /// <inheritdoc />
    public SubmissionResultView(LessonSubmission submission, Enrollment enrollment, IFileStorageService fileStorageService) : base(enrollment.IsSelfStudy, enrollment.Id)
    {
        Lesson = new LessonView(submission.Lesson, enrollment, fileStorageService);
        var answerList = new List<Shared.Enrollments.AnswerView>();
        if (submission.SubmissionDate.HasValue) SubmissionDate = submission.SubmissionDate.Value;
        if (submission.Imported == false && submission.Answers != null)
        {
            foreach (var answer in submission.Answers)
            {
                var question = submission.Lesson.Content.FirstOrDefault(c => c.Id == answer.QuestionId && c.Type == answer.QuestionType);
                if (question == null) continue;
                switch (answer.QuestionType)
                {
                    case ContentBlockTypes.QuestionBoolean:
                        answerList.Add(new Shared.Enrollments.AnswerView(answer, question as BoolQuestion));
                        break;
                    case ContentBlockTypes.QuestionOpen:
                        answerList.Add(new Shared.Enrollments.AnswerView(answer, question as OpenQuestion));
                        break;
                    case ContentBlockTypes.QuestionMultipleChoice:
                        answerList.Add(new Shared.Enrollments.AnswerView(answer, question as MultipleChoiceQuestion));
                        break;
                }
            }
        }
        Answers = answerList.AsReadOnly();
    }

    #endregion
}