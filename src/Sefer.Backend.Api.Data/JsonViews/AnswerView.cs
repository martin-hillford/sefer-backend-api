// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.JsonViews;

/// <summary>
/// A model to hold the corrected answers on the questions
/// </summary>
public class AnswerView
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
    /// The answer given (pure text)
    /// </summary>
    public string TextAnswer { get; set; }

    /// <summary>
    /// Contains the header of question this answer is about
    /// </summary>
    public string QuestionNumber { get; set; }

    /// <summary>
    /// Contains the header of question this answer is about
    /// </summary>
    public string QuestionHeading { get; set; }
    
    /// <summary>
    /// Contains the text of question this answer is about
    /// </summary>
    public string QuestionText { get; set; }

    /// <summary>
    /// Holds the type of the question
    /// </summary>
    /// <returns></returns>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes QuestionType { get; set; }

    /// <summary>
    /// In the cases of a multiple choices question, the choices are set.
    /// So that javascript can match the id's to the choices
    /// </summary>
    public Dictionary<int, string> QuestionChoices { get; set; }

    /// <summary>
    /// Holds if the content is mark down content
    /// </summary>
    public bool IsMarkDownContent { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new view
    /// </summary>
    /// <param name="answer"></param>
    /// <param name="question"></param>
    public AnswerView(QuestionAnswer answer, Question question)
    {
        QuestionId = answer.QuestionId;
        SubmissionId = answer.SubmissionId;
        TextAnswer = answer.TextAnswer;

        QuestionHeading = question.Heading;
        QuestionNumber = question.Number;
        QuestionText = question.Content;
        QuestionType = answer.QuestionType;
        IsMarkDownContent = question.IsMarkDownContent;



        if (question is not MultipleChoiceQuestion mcQuestion) return;
        QuestionChoices = mcQuestion.Choices.Select(c => new { c.Id, c.Answer }).ToDictionary(c => c.Id, c => c.Answer);
    }

    /// <summary>
    /// Constructor for serialization
    /// </summary>
    [JsonConstructor]
    public AnswerView() { }

    #endregion
}