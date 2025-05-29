// This post model is part of the lesson post model. So some of it properties will not be set in the code base 
// ReSharper disable ClassNeverInstantiated.Global, CollectionNeverUpdated.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Lesson;

/// <summary>
/// ContentBlock is a class that is a combination of all content blocks that can be posted.
/// </summary>
public class ContentBlockPostModel
{
    /// <summary>
    /// (Optional) The id of the content block of a lessons, needs to be set when updating a content block
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets / Sets the sequence number for this ContentBlock. Does not have to be unique.
    /// But it will be used for sorting the lessons within a Lesson
    /// </summary>
    [Required]
    public int SequenceId { get; set; }

    /// <summary>
    /// Gets the type of content
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes Type { get; set; }

    /// <summary>
    /// A number for this element. Can be used to separate a lesson in parts (optional).
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// A heading for this element (optional).
    /// </summary>
    public string Heading { get; set; }

    /// <summary>
    /// When true, a page break will be forced for the user
    /// </summary>
    [Required]
    public bool ForcePageBreak { get; set; }

    /// <summary>
    /// Reference to external content i.e. a youtube link (optional). (for a media element)
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Content for this element, containing the question asked the student
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Holds if the content is mark down content
    /// </summary>
    public bool IsMarkDownContent { get; set; }

    /// <summary>
    /// Get or set if the correct answer for the question is true (for a boolean question)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BoolAnswers? Answer { get; set; }

    /// <summary>
    /// Gets / sets if the user can select multiple choices as being correct (for a multiple choice question)
    /// </summary>
    public bool IsMultiSelect { get; set; }

    /// <summary>
    /// Gets / sets the choices when this is multiple choice question
    /// </summary>
    public List<ChoicePostModel> Choices { get; set; }

    /// <summary>
    /// An empty constructor initializing the choices list (empty)
    /// </summary>
    public ContentBlockPostModel()
    {
        Choices = new List<ChoicePostModel>();
    }

    /// <summary>
    /// Converts the ContentBlock into a LessonTextElement
    /// </summary>
    /// <returns>the Element when the type is text, else null</returns>
    public LessonTextElement ToTextElement()
    {
        if (Type != ContentBlockTypes.ElementText) return null;
        return new LessonTextElement
        {
            Type = ContentBlockTypes.ElementText,
            Content = Content,
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            Id = GetId(),
            ModificationDate = DateTime.UtcNow,
            Number = Number,
            SequenceId = SequenceId,
            IsMarkDownContent = IsMarkDownContent
        };
    }

    /// <summary>
    /// Converts the ContentBlock into a LessonTextElement
    /// </summary>
    /// <returns>the Element when the type is a media type, else null</returns>
    public MediaElement ToMediaElement()
    {
        // check for a supported type
        switch (Type)
        {
            case ContentBlockTypes.ElementImage:
            case ContentBlockTypes.ElementAudio:
            case ContentBlockTypes.ElementLink:
            case ContentBlockTypes.ElementVideo:
            case ContentBlockTypes.ElementYoutube:
            case ContentBlockTypes.ElementVimeo:
                break;
            default:
                return null;
        }

        return new MediaElement
        {
            Type = Type,
            Content = Content,
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            Id = GetId(),
            ModificationDate = DateTime.UtcNow,
            Number = Number,
            SequenceId = SequenceId,
            Url = Url,
            IsMarkDownContent = IsMarkDownContent
        };
    }

    /// <summary>
    /// Converts the ContentBlock into an Open Question
    /// </summary>
    /// <returns>the Question when the type is an open question type, else null</returns>
    public OpenQuestion ToOpenQuestion()
    {
        if (Type != ContentBlockTypes.QuestionOpen) return null;
        return new OpenQuestion
        {
            Content = Content,
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            Id = GetId(),
            ModificationDate = DateTime.UtcNow,
            Number = Number,
            SequenceId = SequenceId,
            IsMarkDownContent = IsMarkDownContent
        };
    }

    /// <summary>
    /// Converts the ContentBlock into a Boolean Question
    /// </summary>
    /// <returns>the Question when the type is a boolean question type, else null</returns>
    public BoolQuestion ToBoolQuestion()
    {
        if (Type != ContentBlockTypes.QuestionBoolean) return null;
        if (Answer.HasValue == false) return null;
        var value = Answer.Value;
        return new BoolQuestion
        {
            Content = Content,
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            Id = GetId(),
            ModificationDate = DateTime.UtcNow,
            Number = Number,
            SequenceId = SequenceId,
            CorrectAnswer = value,
            IsMarkDownContent = IsMarkDownContent
        };
    }

    /// <summary>
    /// Converts the ContentBlock into a Multiple Choice Question
    /// </summary>
    /// <returns>the Question when the type is a multiple choice question type, else null</returns>
    public MultipleChoiceQuestion ToMultipleChoiceQuestion()
    {
        if (Type != ContentBlockTypes.QuestionMultipleChoice) return null;
        return new MultipleChoiceQuestion
        {
            Content = Content,
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            Id = GetId(),
            ModificationDate = DateTime.UtcNow,
            Number = Number,
            SequenceId = SequenceId,
            IsMultiSelect = IsMultiSelect,
            Choices = GetChoices(),
            IsMarkDownContent = IsMarkDownContent
        };
    }

    /// <summary>
    /// Converts the null-able id into a zero-able id.
    /// </summary>
    /// <returns></returns>
    private int GetId() => Id ?? 0;

    /// <summary>
    /// Creates a list of the choices that are provided (for a multiple choice question)
    /// </summary>
    private List<MultipleChoiceQuestionChoice> GetChoices()
    {
        var list = new List<MultipleChoiceQuestionChoice>();
        Choices.ForEach(c => list.Add(c.ToChoice()));
        return list;
    }
}
