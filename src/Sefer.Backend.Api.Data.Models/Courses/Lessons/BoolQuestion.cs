// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Courses.Lessons;

/// <summary>
/// A boolean question, just a question with a true or false answer
/// </summary>
/// <inheritdoc cref="Question" />
/// <inheritdoc cref="ILessonQuestion{TLesson,TQuestion}" />
public class BoolQuestion : Question, ILessonQuestion<Lesson, BoolQuestion>
{
    #region Properties

    /// <summary>
    /// Get or set if the correct answer for the question is true
    /// </summary>
    public bool CorrectAnswerIsTrue { get; set; }

    /// <summary>
    /// The text value for a correct answers
    /// </summary>
    public const string Correct = "Correct";

    /// <summary>
    /// The text value for a wrong answers
    /// </summary>
    public const string Wrong = "Wrong";

    /// <summary>
    /// Get or set if the correct answer for the question is true
    /// </summary>
    [NotMapped]
    public BoolAnswers CorrectAnswer
    {
        get => CorrectAnswerIsTrue ? BoolAnswers.Correct : BoolAnswers.Wrong;
        set => CorrectAnswerIsTrue = value == BoolAnswers.Correct;
    }

    /// <summary>
    /// Gets a text version of the correct answer
    /// </summary>
    [NotMapped]
    public string CorrectAnswerText => CorrectAnswerIsTrue ? Correct : Wrong;

    /// <summary>
    /// The lesson to which the multiple choice question belongs
    /// </summary>
    /// <inheritdoc />
    [ForeignKey("LessonId")]
    public Lesson Lesson { get; set; }

    /// <summary>
    /// Returns a header for this open question
    /// </summary>
    /// <inheritdoc />
    public string HeaderText => Element.GetHeaderText(this);

    /// <summary>
    /// The type of the question
    /// </summary>
    /// <inheritdoc />
    public ContentBlockTypes Type => ContentBlockTypes.QuestionBoolean;

    /// <summary>
    /// The predecessor for this BoolQuestion
    /// </summary>
    [ForeignKey("PredecessorId")]
    public BoolQuestion Predecessor { get; set; }

    /// <summary>
    /// Returns if the content block is question
    /// </summary>
    public bool IsQuestion => true;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a successor for this element given a lesson
    /// </summary>
    /// <param name="lesson"></param>
    /// <returns></returns>
    /// <inheritdoc />
    public BoolQuestion CreateSuccessor(Lesson lesson)
    {
        var successor = new BoolQuestion
        {
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            LessonId = lesson.Id,
            Number = Number,
            PredecessorId = Id,
            SequenceId = SequenceId,
            Content = Content,
            CorrectAnswer = CorrectAnswer,
            CorrectAnswerIsTrue = CorrectAnswerIsTrue,
            IsMarkDownContent = IsMarkDownContent,
            AnswerExplanation = AnswerExplanation
        };
        return successor;
    }

    #endregion
}