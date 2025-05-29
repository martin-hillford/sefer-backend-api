// ReSharper disable ClassNeverInstantiated.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Student.Profile;

/// <summary>
/// The model for an answer on a question
/// </summary>
public class QuestionAnswerPostModel
{
    #region Properties

    /// <summary>
    /// The id of the question this answer applies to
    /// </summary>
    public int QuestionId { get; set; }

    /// <summary>
    /// The answer on the question
    /// </summary>
    /// <returns></returns>
    public string Answer { get; set; }

    /// <summary>
    /// Holds the type of the question
    /// </summary>
    /// <returns></returns>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes QuestionType { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// This method simply converts from a model to a db model
    /// </summary>
    /// <returns></returns>
    public QuestionAnswer Convert()
    {
        return new QuestionAnswer
        {
            TextAnswer = Answer,
            QuestionId = QuestionId,
            QuestionType = QuestionType
        };
    }

    #endregion
}