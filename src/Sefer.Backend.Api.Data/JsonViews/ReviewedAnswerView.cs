namespace Sefer.Backend.Api.Data.JsonViews;

/// <summary>
/// A model to hold the reviewed answers on the questions
/// </summary>
public class ReviewedAnswerView : AnswerView
{
    /// <summary>
    /// The correct answer that should be given
    /// </summary>
    [JsonInclude]
    public string CorrectAnswer { get; set; }

    /// <summary>
    /// Gets if the answer is correct. For open questions this is not possible of course
    /// </summary>
    [JsonInclude]
    public bool? IsValid { get; set; }

    /// <summary>
    /// Gets set's if the mentor has given a review
    /// </summary>
    [JsonInclude]
    public string MentorReview { get; set; }

    public ReviewedAnswerView(QuestionAnswer answer, Question question) : base(answer, question)
    {
        IsValid = answer.IsCorrectAnswer;
        MentorReview = answer.MentorReview;
    }

    /// <summary>
    /// Creates a new view
    /// </summary>
    public ReviewedAnswerView(QuestionAnswer answer, BoolQuestion question) : this(answer, (Question)question)
    {
        CorrectAnswer = question.CorrectAnswerText;
    }

    /// <summary>
    /// Creates a new view
    /// </summary>
    public ReviewedAnswerView(QuestionAnswer answer, OpenQuestion question) : this(answer, (Question)question)
    {
        CorrectAnswer = string.Empty;
    }

    /// <summary>
    /// Creates a new view
    /// </summary>
    public ReviewedAnswerView(QuestionAnswer answer, MultipleChoiceQuestion question) : this(answer, (Question)question)
    {
        CorrectAnswer = string.Join(',', question.Choices.Where(c => c.IsCorrectAnswer).Select(c => c.Id.ToString()));
    }

    /// <summary>
    /// Constructor for serialization
    /// </summary>
    public ReviewedAnswerView() { }
}