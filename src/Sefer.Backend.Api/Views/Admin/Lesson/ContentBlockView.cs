// This is a view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Views.Admin.Lesson;

/// <summary>
/// ContentBlock is a class that is a combination of all content blocks.
/// Using this structure will make it easier to post since it matches that post model
/// </summary>
public class ContentBlockView
{
    #region Properties

    /// <summary>
    /// A base content block
    /// </summary>
    private readonly ContentBlock _block;

    /// <summary>
    /// The id of the content block.
    /// </summary>
    public int Id => _block.Id;

    /// <summary>
    /// Gets the sequence number for this ContentBlock. Does have to be unique.
    /// And it will be used for sorting the lessons within a Lesson
    /// </summary>
    public int SequenceId => _block.SequenceId;

    /// <summary>
    /// Gets the type of content
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public readonly ContentBlockTypes Type;

    /// <summary>
    /// A number for this element. Can be used to separate a lesson in parts.
    /// </summary>
    public string Number => _block.Number;

    /// <summary>
    /// A heading for this element.
    /// </summary>
    public string Heading => _block.Heading;

    /// <summary>
    /// A created header for this element.
    /// </summary>
    public string Header
    {
        get
        {
            var header = "";
            if (Number != "") header += Number + ": ";
            if (Heading != "") header += Heading;
            return header;
        }
    }

    /// <summary>
    /// When true, a page break will be forced for the user
    /// </summary>
    [Required]
    public bool ForcePageBreak => _block.ForcePageBreak;

    /// <summary>
    /// Reference to external content i.e. a youtube link (optional). (for a media element)
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Content for this element, containing the question in case of a question, or any additional content in case of a media element
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Holds if the content is mark-down content
    /// </summary>
    public bool IsMarkDownContent { get; set; }

    /// <summary>
    /// Get or set if the correct answer for the question is true (for a boolean question)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BoolAnswers? Answer { get; set; }

    /// <summary>
    /// Gets / sets if the user can select multiple choices as being correct (for a multiple-choice question)
    /// </summary>
    public bool IsMultiSelect { get; set; }
    
    /// <summary>
    /// An open question can have an exact answer (for example, when a single word must be given)
    /// This will not be shown to students on default
    /// </summary>
    public string ExactAnswer { get; set; }
    
    /// <summary>
    /// A course maker or admin can add an explanation to the question explaining what is the good answer.
    /// This explanation is not shown by default
    /// </summary>
    public string AnswerExplanation { get; set; }

    /// <summary>
    /// Gets if this element is a question
    /// </summary>
    public bool IsQuestion =>
        Type == ContentBlockTypes.QuestionBoolean ||
        Type == ContentBlockTypes.QuestionMultipleChoice ||
        Type == ContentBlockTypes.QuestionOpen;

    /// <summary>
    /// Gets if this element is a media element
    /// </summary>
    public bool IsMedia =>
        Type == ContentBlockTypes.ElementAudio ||
        Type == ContentBlockTypes.ElementImage ||
        Type == ContentBlockTypes.ElementLink ||
        Type == ContentBlockTypes.ElementVideo ||
        Type == ContentBlockTypes.ElementYoutube;

    /// <summary>
    /// Gets / sets the choices when this is a multiple-choice question
    /// </summary>
    // ReSharper disable once CollectionNeverQueried.Global
    public List<ChoiceView> Choices { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// A model constructor initializing the list with choices
    /// </summary>
    private ContentBlockView(ContentBlock block, ContentBlockTypes type)
    {
        _block = block;
        Choices = [];
        Type = type;
    }

    /// <summary>
    /// Creates a view for an open question
    /// </summary>
    /// <param name="element"></param>
    /// <inheritdoc />
    public ContentBlockView(LessonTextElement element) : this(element, element.Type)
    {
        Content = element.Content;
        IsMarkDownContent = element.IsMarkDownContent;
    }

    /// <summary>
    /// Creates a view for an open question
    /// </summary>
    /// <param name="element"></param>
    /// <inheritdoc />
    public ContentBlockView(MediaElement element) : this(element, element.Type)
    {
        Content = element.Content;
        Url = element.Url;
        IsMarkDownContent = element.IsMarkDownContent;
    }

    /// <summary>
    /// Creates a view for an open question
    /// </summary>
    /// <param name="question"></param>
    /// <inheritdoc />
    public ContentBlockView(OpenQuestion question) : this(question, question.Type)
    {
        Content = question.Content;
        IsMarkDownContent = question.IsMarkDownContent;
        ExactAnswer = question.ExactAnswer;
        AnswerExplanation = question.AnswerExplanation;
    }

    /// <summary>
    /// Creates a view for a boolean question
    /// </summary>
    /// <param name="question"></param>
    /// <inheritdoc />
    public ContentBlockView(BoolQuestion question) : this(question, question.Type)
    {
        Content = question.Content;
        Answer = question.CorrectAnswer;
        IsMarkDownContent = question.IsMarkDownContent;
        AnswerExplanation = question.AnswerExplanation;
    }

    /// <summary>
    /// Creates a view for a boolean question
    /// </summary>
    /// <param name="question"></param>
    /// <inheritdoc />
    public ContentBlockView(MultipleChoiceQuestion question) : this(question, question.Type)
    {
        Content = question.Content;
        IsMultiSelect = question.IsMultiSelect;
        IsMarkDownContent = question.IsMarkDownContent;
        AnswerExplanation = question.AnswerExplanation;
        foreach (var choice in question.Choices)
        {
            Choices.Add(new ChoiceView(choice));
        }
    }

    #endregion
}