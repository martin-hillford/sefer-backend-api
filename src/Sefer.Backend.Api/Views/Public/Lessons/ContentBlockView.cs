// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable CollectionNeverQueried.Global
namespace Sefer.Backend.Api.Views.Public.Lessons;

/// <summary>
/// ContentBlock is a class that is a combination of all content blocks.
/// Using this structure will make it easier to post since it matches that post model
/// </summary>
/// Todo: Refactor
public class ContentBlockView
{
    /// <summary>
    /// A base content block
    /// </summary>
    private readonly ContentBlock _block;

    /// <summary>
    /// The id of the content block of a lessons.
    /// </summary>
    public int Id => _block.Id;

    /// <summary>
    /// Holds if the content is mark down content
    /// </summary>
    public bool IsMarkDownContent { get; set; }

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
    /// a created header for this element.
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
    /// Text for this element, containing the question asked the student
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets / sets if the user can select multiple choices as being correct (for a multiple choice question)
    /// </summary>
    public bool IsMultiSelect { get; set; }

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
    /// Gets / sets the choices when this is multiple choice question
    /// </summary>
    public List<ChoiceView> Choices { get; set; }

    /// <summary>
    /// A model constructor initializing the choices list
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
        Text = element.Content;
        IsMarkDownContent = element.IsMarkDownContent;
    }

    /// <summary>
    /// Creates a view for an open question
    /// </summary>
    /// <param name="element"></param>
    /// <inheritdoc />
    public ContentBlockView(MediaElement element) : this(element, element.Type)
    {
        Text = element.Content;
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
        Text = question.Content;
        IsMarkDownContent = question.IsMarkDownContent;
    }

    /// <summary>
    /// Creates a view for a boolean question
    /// </summary>
    /// <param name="question"></param>
    /// <inheritdoc />
    public ContentBlockView(BoolQuestion question) : this(question, question.Type)
    {
        Text = question.Content;
        IsMarkDownContent = question.IsMarkDownContent;
    }

    /// <summary>
    /// Creates a view for a boolean question
    /// </summary>
    /// <param name="question"></param>
    /// <inheritdoc />
    public ContentBlockView(MultipleChoiceQuestion question) : this(question, question.Type)
    {
        Text = question.Content;
        IsMultiSelect = question.IsMultiSelect;
        foreach (var choice in question.Choices)
        {
            Choices.Add(new ChoiceView(choice));
            IsMarkDownContent = question.IsMarkDownContent;
        }
    }
}