// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Enrollments;

/// <summary>
/// An answer is an answer on a question within the context of submission
/// </summary>
/// <inheritdoc cref="ModifyDateLogEntity"/>
public class QuestionAnswer : ModifyDateLogEntity
{
    #region Properties

    /// <summary>
    /// The identifier of the question that is answered
    /// </summary>
    /// <returns></returns>
    public int QuestionId { set; get; }

    /// <summary>
    /// The identifier of the submission that this answer belongs to
    /// </summary>
    public int SubmissionId { set; get; }

    /// <summary>
    /// The answer on other question types
    /// </summary>
    public string TextAnswer { set; get; }

    /// <summary>
    /// Holds the type of the question
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes QuestionType { get; set; }

    /// <summary>
    /// Holds if the answer is correct, but only for final submitted answers
    /// </summary>
    /// <returns></returns>
    public bool? IsCorrectAnswer { get; set; }

    /// <summary>
    /// Optional review of the mentor on the answer
    /// </summary>
    public string MentorReview { get; set; }

    #endregion

    #region References

    /// <summary>
    /// A reference to the submission
    /// </summary>
    [JsonIgnore, ForeignKey("SubmissionId")]
    public LessonSubmission Submission { get; set; }

    /// <summary>
    /// The answer on a boolean question
    /// </summary>
    [NotMapped]
    public bool? BoolAnswer
    {
        get
        {
            if (QuestionType != ContentBlockTypes.QuestionBoolean) return null;
            var value = TextAnswer.ToLower();
            return value == "correct" || value == "true" || value == "1";
        }
    }
    
    #endregion
}