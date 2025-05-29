// This post model is part of the lesson post model. So some of it properties will not be set in the code base 
// ReSharper disable ClassNeverInstantiated.Global, CollectionNeverUpdated.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Lesson;

/// <summary>
/// This class is used to post the choice of a multiple choice question
/// </summary>
public class ChoicePostModel
{
    /// <summary>
    /// (Optional) The id of the choice of a multiple choice question, needs to be set when updating
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets / sets the answer of the choice
    /// </summary>
    [Required]
    public string Answer { get; set; }

    /// <summary>
    /// Gets / sets if the answer is correct for the choice
    /// </summary>
    [Required]
    public bool IsCorrectAnswer { get; set; }

    /// <summary>
    /// Converts the null-able id into a zero-able id.
    /// </summary>
    /// <returns></returns>
    private int GetId() => Id ?? 0;

    /// <summary>
    /// Converts this choice into a model object
    /// </summary>
    public MultipleChoiceQuestionChoice ToChoice()
    {
        return new MultipleChoiceQuestionChoice
        {
            Id = Math.Max(0, GetId()),
            Answer = Answer,
            IsCorrectAnswer = IsCorrectAnswer
        };
    }
}
