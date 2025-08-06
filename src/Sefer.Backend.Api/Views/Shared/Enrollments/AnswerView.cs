// This is a view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Shared.Enrollments;

/// <summary>
/// A model to hold the corrected answers on the questions
/// </summary>
public class AnswerView : AbstractView<QuestionAnswer>
{
    /// <summary>
    /// The id of the question that is answered
    /// </summary>
    /// <returns></returns>
    public int QuestionId => Model.QuestionId;

    /// <summary>
    /// This is the submitted version of the text
    /// </summary>
    public readonly string GivenAnswer;

    /// <summary>
    /// Contains the number of the question this answer is about
    /// </summary>
    public readonly string QuestionNumber;

    /// <summary>
    /// Contains the header of the question this answer is about
    /// </summary>
    public readonly string QuestionHeading;

    /// <summary>
    /// A created header for this element.
    /// </summary>
    public string QuestionHeader
    {
        get
        {
            var header = "";
            if (QuestionNumber != "") header += QuestionNumber + ": ";
            if (QuestionHeading != "") header += QuestionHeading;
            return header;
        }
    }

    /// <summary>
    /// Contains the text of the question this answer is about
    /// </summary>
    public readonly string QuestionText;

    /// <summary>
    /// Holds if the content is mark-down content
    /// </summary>
    public bool IsMarkDownContent;

    /// <summary>
    /// Holds the type of the question
    /// </summary>
    /// <returns></returns>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes QuestionType => Model.QuestionType;

    /// <summary>
    /// In the cases of a multiple choices question, the choices are set.
    /// So that JavaScript can match the id's to the choices
    /// </summary>
    public readonly IReadOnlyDictionary<int, string> QuestionChoices;

    /// <summary>
    /// The sequenceId is used to determine the sequence of the answers (based on the sequence of the question)
    /// </summary>
    public readonly int SequenceId;

    /// <summary>
    /// Creates a new view
    /// </summary>
    /// <param name="answer"></param>
    /// <param name="question"></param>
    public AnswerView(QuestionAnswer answer, BoolQuestion question) : base(answer)
    {
        GivenAnswer = answer.TextAnswer;
        QuestionHeading = question.Heading;
        QuestionNumber = question.Number;
        QuestionText = question.Content;
        SequenceId = question.SequenceId;
        IsMarkDownContent = question.IsMarkDownContent;
    }

    /// <summary>
    /// Creates a new view
    /// </summary>
    /// <param name="answer"></param>
    /// <param name="question"></param>
    public AnswerView(QuestionAnswer answer, OpenQuestion question) : base(answer)
    {
        GivenAnswer = answer.TextAnswer;
        QuestionHeading = question.Heading;
        QuestionNumber = question.Number;
        QuestionText = question.Content;
        SequenceId = question.SequenceId;
        IsMarkDownContent = question.IsMarkDownContent;
    }

    /// <summary>
    /// Creates a new view
    /// </summary>
    /// <param name="answer"></param>
    /// <param name="question"></param>
    public AnswerView(QuestionAnswer answer, MultipleChoiceQuestion question) : base(answer)
    {
        GivenAnswer = answer.TextAnswer;
        QuestionHeading = question.Heading;
        QuestionNumber = question.Number;
        QuestionText = question.Content;
        QuestionChoices = question.Choices.Select(c => new { c.Id, c.Answer }).ToDictionary(c => c.Id, c => c.Answer);
        SequenceId = question.SequenceId;
        IsMarkDownContent = question.IsMarkDownContent;
    }
}