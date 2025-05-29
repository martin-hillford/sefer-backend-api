// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Shared.Courses.Lessons;

/// <summary>
/// This view is be used for admins, mentors and supervisors to provide a full overview of the questions in a course
/// </summary>
public class LessonCheckQuestionView : AbstractView<Lesson>
{
    #region Properties

    /// <summary>
    /// Gets / Sets the sequence number for this lesson. Does not have to be unique.
    /// But it will be used for sorting the lessons within a course revision
    /// </summary>
    public int SequenceId => Model.SequenceId;

    /// <summary>
    /// Gets / sets the chapter / section number for this lesson
    /// </summary>
    public string Number => Model.Number;

    /// <summary>
    ///  Gets / sets name for this lesson
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// Returns a simple header for the lesson (western languages)
    /// </summary>
    public string Header => Model.Number + ":&nbsp;" + Model.Name;

    /// <summary>
    /// Gets / sets a description for this lesson
    /// </summary>
    public string Description => Model.Description;

    /// <summary>
    /// Gets / sets what the student should read before the start of the lesson
    /// </summary>
    public string ReadBeforeStart => Model.ReadBeforeStart;

    /// <summary>
    /// Gets / sets the id of the <see cref="Sefer.Backend.Api.Data.Models.Courses.CourseRevision"/>  this lesson belongs to
    /// </summary>
    public int CourseRevisionId => Model.CourseRevisionId;

    /// <summary>
    /// A list of questions within this lesson
    /// </summary>
    public readonly ReadOnlyCollection<QuestionCheckView> Questions;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="questions">The list of questions for this lesson</param>
    /// <inheritdoc />
    public LessonCheckQuestionView(Lesson model, List<Question> questions) : base(model)
    {
        var questionsList = new List<QuestionCheckView>();
        foreach (var question in questions)
        {
            switch (question)
            {
                case OpenQuestion openQuestion:
                    questionsList.Add(new QuestionCheckView(openQuestion));
                    break;
                case BoolQuestion booleanQuestion:
                    questionsList.Add(new QuestionCheckView(booleanQuestion));
                    break;
                case MultipleChoiceQuestion multipleChoiceQuestion:
                    questionsList.Add(new QuestionCheckView(multipleChoiceQuestion));
                    break;
            }
        }
        Questions = questionsList.AsReadOnly();
    }

    #endregion
}