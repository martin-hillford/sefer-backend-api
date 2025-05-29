namespace Sefer.Backend.Api.Data.Models.Abstractions;

/// <summary>
/// Defines an interface for question of lesson (basically a combination of a lesson and a content block)
/// </summary>
/// <typeparam name="TSurvey"></typeparam>
/// <typeparam name="TQuestion"></typeparam>
/// <inheritdoc cref="IQuestion"/>
public interface ISurveyQuestion<in TSurvey, out TQuestion> :
    IQuestion where TSurvey : Survey where TQuestion : ISurveyQuestion<TSurvey, TQuestion>
{
    /// <summary>
    /// Create a successor for the question given the survey
    /// </summary>
    /// <param name="survey"></param>
    /// <returns></returns>
    TQuestion CreateSuccessor(TSurvey survey);
}