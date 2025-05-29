// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Mentor;

/// <summary>
/// The information of the user to update his profile
/// </summary>
public class SubmissionReviewPostModel
{
    /// <summary>
    /// The id of the submission this review is about
    /// </summary>
    public int Id { get; set;}

    /// <summary>
    /// When set the true the review is final and is the results are send to the student
    /// </summary>
    public bool Final { get; set;}

    /// <summary>
    /// The list of reviews on the answers of the student
    /// </summary>
    public List<SubmissionReviewAnswerPostModel> Answers { get; set;}
}