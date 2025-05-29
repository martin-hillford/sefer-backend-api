// This is a post model. And although it is never instantiated in code, it is in runtime 
// ReSharper disable UnusedAutoPropertyAccessor.Global ClassNeverInstantiated.Global
namespace Sefer.Backend.Api.Models.Mentor;

/// <summary>
/// The information of the user to update his profile
/// </summary>
public class SubmissionReviewAnswerPostModel
{
    /// <summary>
    /// The Id of the answer this review is about
    /// </summary>
    public int Id { get; set;}

    /// <summary>
    /// The comment of the mentor on the answer of the student (optional)
    /// </summary>
    public string Review { get; set;}
}