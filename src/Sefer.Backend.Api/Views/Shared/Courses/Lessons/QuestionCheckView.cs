// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global UnusedAutoPropertyAccessor.Global 
using Sefer.Backend.Api.Views.Admin.Lesson;

namespace Sefer.Backend.Api.Views.Shared.Courses.Lessons;

/// <summary>
/// This view is be used for admins, mentors and supervisors to provide a full overview of the questions in a course
/// </summary>
public class QuestionCheckView
{
    /// <summary>
    /// A base question
    /// </summary>
    private readonly Question _question;

    /// <summary>
    /// The id of the question of a lessons.
    /// </summary>
    public int Id => _question.Id;

    /// <summary>
    /// Gets the sequence number for this question.Has to be unique.
    /// And it will be used for sorting the lessons within a Lesson
    /// </summary>
    public int SequenceId => _question.SequenceId;

    /// <summary>
    /// Gets the type of the question
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public readonly ContentBlockTypes Type;

    /// <summary>
    /// A number for this element. Can be used to separate a lesson in parts.
    /// </summary>
    public string Number => _question.Number;

    /// <summary>
    /// A heading for this element.
    /// </summary>
    public string Heading => _question.Heading;

    /// <summary>
    /// When true, a page break will be forced for the user
    /// </summary>
    public bool ForcePageBreak => _question.ForcePageBreak;

    /// <summary>
    /// Text for this element, containing the question asked the student
    /// </summary>
    public string Content => _question.Content;

    /// <summary>
    /// Gets / sets if the user can select multiple choices as being correct (for a multiple choice question)
    /// </summary>
    public bool IsMultiSelect { get; set; }

    /// <summary>
    /// Gets / sets the choices when this is multiple choice question
    /// </summary>
    public List<ChoiceView> Choices { get; set; }

    /// <summary>
    /// Holds if this is a markdown block
    /// </summary>
    public bool IsMarkDownContent => _question.IsMarkDownContent;

    /// <summary>
    /// Contains the correct answer for the question.
    /// For open question this is an empty string
    /// For boolean question this is the word 'Correct' or 'Wrong' (the text representation within the bool question object)
    /// For multiple choice questions this is a comma separated list of choice id's
    /// </summary>
    public readonly string Answer;

    /// <summary>
    /// Create a view for a boolean question
    /// </summary>
    /// <param name="boolQuestion"></param>
    public QuestionCheckView(BoolQuestion boolQuestion)
    {
        _question = boolQuestion;
        Choices = new List<ChoiceView>();
        IsMultiSelect = false;
        Type = ContentBlockTypes.QuestionBoolean;
        Answer = boolQuestion.CorrectAnswerText;
    }

    /// <summary>
    /// Create a view for a boolean question
    /// </summary>
    /// <param name="openQuestion"></param>
    public QuestionCheckView(OpenQuestion openQuestion)
    {
        _question = openQuestion;
        Choices = new List<ChoiceView>();
        IsMultiSelect = false;
        Type = ContentBlockTypes.QuestionOpen;
        Answer = string.Empty;
    }

    /// <summary>
    /// Create a view for a boolean question
    /// </summary>
    /// <param name="multipleChoiceQuestion"></param>
    public QuestionCheckView(MultipleChoiceQuestion multipleChoiceQuestion)
    {
        _question = multipleChoiceQuestion;
        Choices = multipleChoiceQuestion.Choices.Select(c => new ChoiceView(c)).ToList();
        IsMultiSelect = multipleChoiceQuestion.IsMultiSelect;
        Type = ContentBlockTypes.QuestionMultipleChoice;
        Answer = string.Join(',', multipleChoiceQuestion.Choices.Where(c => c.IsCorrectAnswer).Select(c => c.Id.ToString()));
    }
}