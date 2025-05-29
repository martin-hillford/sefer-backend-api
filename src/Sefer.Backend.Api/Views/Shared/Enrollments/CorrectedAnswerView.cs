// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Shared.Enrollments;

/// <summary>
/// A model to hold the corrected answers on the questions
/// </summary>
public class CorrectedAnswerView : AnswerView
{
    /// <summary>
    /// The correct answer that should be given
    /// </summary>
    public readonly string CorrectAnswer;

    /// <summary>
    /// Gets if the answer is correct. For open questions this is not possible of course
    /// </summary>
    public readonly bool? IsValid;

    /// <summary>
    /// Optional review of the mentor on the answer
    /// </summary>
    public string MentorReview => Model.MentorReview;

    /// <summary>
    /// Creates a new view
    /// </summary>
    /// <param name="answer"></param>
    /// <param name="question"></param>
    public CorrectedAnswerView(QuestionAnswer answer, BoolQuestion question) : base(answer, question)
    {
        CorrectAnswer = question.CorrectAnswerText;
        IsValid = answer.IsCorrectAnswer;
    }

    /// <summary>
    /// Creates a new view
    /// </summary>
    /// <param name="answer"></param>
    /// <param name="question"></param>
    public CorrectedAnswerView(QuestionAnswer answer, OpenQuestion question) : base(answer, question)
    {
        CorrectAnswer = string.Empty;
    }

    /// <summary>
    /// Creates a new view
    /// </summary>
    /// <param name="answer"></param>
    /// <param name="question"></param>
    public CorrectedAnswerView(QuestionAnswer answer, MultipleChoiceQuestion question) : base(answer, question)
    {
        CorrectAnswer = string.Join(',', question.Choices.Where(c => c.IsCorrectAnswer).Select(c => c.Id.ToString()));
        IsValid = answer.IsCorrectAnswer;
    }
}