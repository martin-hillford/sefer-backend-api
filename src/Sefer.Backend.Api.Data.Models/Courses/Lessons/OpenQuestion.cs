﻿namespace Sefer.Backend.Api.Data.Models.Courses.Lessons;

/// <summary>
/// An open question is just a question with a question which the user has
/// to respond with a written text.
/// </summary>
/// <inheritdoc cref="Question"/>
/// <inheritdoc cref="ILessonQuestion{TLesson,TQuestion}"/>
public class OpenQuestion : Question, ILessonQuestion<Lesson, OpenQuestion>
{
    #region Properties

    /// <summary>
    /// The lesson to which the multiple-choice question belongs
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
    /// An open question can have an exact answer (for example, when a single word must be given)
    /// This will not be shown to students on default
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string ExactAnswer { get; set; }
    
    /// <summary>
    /// The type of the Element in
    /// </summary>
    /// <inheritdoc />
    public ContentBlockTypes Type => ContentBlockTypes.QuestionOpen;

    /// <summary>
    /// The predecessor for this OpenQuestion
    /// </summary>
    [ForeignKey("PredecessorId")]
    public OpenQuestion Predecessor { get; set; }
    
    /// <summary>
    /// Returns if the content block is question
    /// </summary>
    public bool IsQuestion => true;

    /// <summary>
    /// When an open-question has an exact answer is gradable.
    /// </summary>
    public bool IsGradable => !string.IsNullOrEmpty(ExactAnswer);
    
    #endregion

    #region Methods

    /// <summary>
    /// Creates a successor for this element given a lesson
    /// </summary>
    /// <param name="lesson"></param>
    /// <returns></returns>
    /// <inheritdoc />
    public OpenQuestion CreateSuccessor(Lesson lesson)
    {
        return new OpenQuestion
        {
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            LessonId = lesson.Id,
            Number = Number,
            PredecessorId = Id,
            SequenceId = SequenceId,
            Content = Content,
            IsMarkDownContent = IsMarkDownContent,
            ExactAnswer = ExactAnswer,
            AnswerExplanation = AnswerExplanation
        };
    }

    #endregion
}