namespace Sefer.Backend.Api.Data.Models.Courses.Lessons;

/// <summary>
/// A Multiple Choice Question is a question for which the user can select one or more
/// answer as being the correct answer
/// </summary>
/// <inheritdoc cref="Question"/>
/// <inheritdoc cref="IMultipleChoiceQuestion{T}"/>
/// <inheritdoc cref="ILessonQuestion{Lesson, MultipleChoiceQuestion}"/>
public class MultipleChoiceQuestion : Question, IMultipleChoiceQuestion<MultipleChoiceQuestionChoice>, ILessonQuestion<Lesson, MultipleChoiceQuestion>
{
    #region Properties

    /// <summary>
    /// Gets / sets if the user can select multiple choices as being correct
    /// </summary>
    /// <inheritdoc />
    [Required]
    [MultiSelectQuestion]
    public bool IsMultiSelect { get; set; }

    /// <summary>
    /// Contains a list of right answer
    /// </summary>
    /// <inheritdoc />
    public ICollection<MultipleChoiceQuestionChoice> Choices { get; set; } = new List<MultipleChoiceQuestionChoice>();

    /// <summary>
    /// The lesson to which the multiple-choice question belongs
    /// </summary>
    /// <inheritdoc />
    [ForeignKey("LessonId")]
    public Lesson Lesson { get; set; }

    /// <summary>
    /// Gets the number of correct answers for this multiple-choice collection
    /// </summary>
    /// <inheritdoc />
    public int CorrectAnswerCount => Choices.Count(c => c.IsCorrectAnswer);

    /// <summary>
    /// Returns a header for this question
    /// </summary>
    /// <inheritdoc />
    public string HeaderText => Element.GetHeaderText(this);

    /// <summary>
    /// Gets the type of the question
    /// </summary>
    /// <inheritdoc />
    [NotMapped]
    public ContentBlockTypes Type => ContentBlockTypes.QuestionMultipleChoice;

    /// <summary>
    /// The predecessor for this MultipleChoiceQuestion
    /// </summary>
    [ForeignKey("PredecessorId")]
    public MultipleChoiceQuestion Predecessor { get; set; }

    /// <summary>
    /// Returns if the content block is question
    /// </summary>
    public bool IsQuestion => true;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a successor for this element given a lesson
    /// (Does not include choices!!)
    /// </summary>
    /// <param name="lesson"></param>
    /// <returns></returns>
    /// <inheritdoc />
    public MultipleChoiceQuestion CreateSuccessor(Lesson lesson)
    {
        var successor = new MultipleChoiceQuestion
        {
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            LessonId = lesson.Id,
            Number = Number,
            PredecessorId = Id,
            SequenceId = SequenceId,
            IsMultiSelect = IsMultiSelect,
            Content = Content,
            IsMarkDownContent = IsMarkDownContent,
            AnswerExplanation = AnswerExplanation
        };
        return successor;
    }

    #endregion
}