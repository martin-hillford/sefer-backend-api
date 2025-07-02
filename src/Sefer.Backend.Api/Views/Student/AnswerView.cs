// This is view, so property may not be accessed in code, but wll be in serialization
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// This is a view on the answer provided by the user
/// </summary>
public class AnswerView : AbstractView<QuestionAnswer>
{
    #region Properties

    /// <summary>
    /// The id of the question that is answered
    /// </summary>
    /// <returns></returns>
    public int QuestionId { get; set; }

    /// <summary>
    /// The id of the submission that this answer belongs to
    /// </summary>
    public int SubmissionId { get; set; }

    /// <summary>
    /// The answer on other question types
    /// </summary>
    public string TextAnswer { get; set; }

    /// <summary>
    /// Holds the type of the question
    /// </summary>
    /// <returns></returns>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes QuestionType { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new view
    /// </summary>
    /// <param name="answer"></param>
    public AnswerView(QuestionAnswer answer) : base(answer)
    {
        QuestionId = answer.QuestionId;
        SubmissionId = answer.SubmissionId;
        TextAnswer = answer.TextAnswer;
        QuestionType = answer.QuestionType;
    }
    
    [JsonConstructor]
    public AnswerView() { }

    #endregion
}