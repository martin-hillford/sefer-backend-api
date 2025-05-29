// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// This view is used to return to the student any submission about a lesson he has made
/// </summary>
public class LessonSubmissionView : AbstractView<LessonSubmission>
{
    #region Properties

    /// <summary>
    /// When the submission is final this property contains the submission date
    /// </summary>
    public DateTime? SubmissionDate => Model.CreationDate;

    /// <summary>
    /// The id of the enrollment the submission is part of
    /// </summary>
    public int EnrollmentId => Model.EnrollmentId;

    /// <summary>
    /// The id of the lesson this submission is about
    /// </summary>
    public int LessonId => Model.LessonId;

    /// <summary>
    /// When set to true, the lesson is completed and can't be edited anymore
    /// </summary>
    public bool IsFinal => Model.IsFinal;

    /// <summary>
    /// Holds a list with answer given by the student
    /// </summary>
    public readonly ReadOnlyCollection<AnswerView> Answers;

    /// <summary>
    /// Holds if the enrollment is imported from the old system
    /// </summary>
    public bool Imported => Model.Imported;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new view
    /// </summary>
    /// <param name="submission"></param>
    public LessonSubmissionView(LessonSubmission submission) : base(submission)
    {
        var answerList = new List<AnswerView>();
        if (submission.Imported == false && submission.Answers != null)
        {
            answerList = submission.Answers.Select(a => new AnswerView(a)).ToList();
        }
        Answers = answerList.AsReadOnly();
    }

    #endregion
}