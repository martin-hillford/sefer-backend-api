namespace Sefer.Backend.Api.Support;

public abstract class ReviewController(IServiceProvider serviceProvider) : GrantController(serviceProvider)
{
    private readonly INotificationService _notificationService = serviceProvider.GetService<INotificationService>();

    protected async Task<bool> ReviewSubmission(SubmissionReviewPostModel review, User reviewer)
    {
        // Get the submission this review is about
        if (reviewer.IsMentor == false && reviewer.Role != UserRoles.Admin) return false;
        var submission = await Send(new GetSubmissionForReviewByIdRequest(reviewer.Id, review.Id));
        if (submission == null || submission.Imported) return false;

        // Check if every answer has a review
        var answerIds = review.Answers.Select(a => a.Id).Union(submission.Answers.Select(a => a.Id));
        if (answerIds.Count() != submission.Answers.Count) return false;

        // Now loop through all the answer review of the mentor and set the review on the mentor
        foreach (var reviewAnswer in review.Answers)
        {
            var answer = submission.Answers.Single(sa => sa.Id == reviewAnswer.Id);
            await Send(new SaveReviewOfAnswerRequest(answer.Id, reviewAnswer.Review));
        }

        // All the answers are now updated, deal with the submission itself when it's final
        if (!review.Final) return true;

        // update the submission
        submission.ResultsStudentVisible = true;
        submission.ReviewDate = DateTime.UtcNow;
        var updated = await Send(new UpdateSubmissionRequest(submission));
        if (!updated) return false;

        // and post a submission message in the channel
        await _notificationService.SendSubmissionReviewedNotificationAsync(submission);

        // The update is done
        return true;
    }
}