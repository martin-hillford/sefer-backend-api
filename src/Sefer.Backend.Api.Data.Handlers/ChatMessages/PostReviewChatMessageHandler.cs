namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class PostReviewChatMessageHandler(IServiceProvider serviceProvider)
    : PostChatMessageHandler<PostReviewChatMessageRequest, List<Message>>(serviceProvider)
{
    public override async Task<List<Message>> Handle(PostReviewChatMessageRequest request, CancellationToken token)
    {
        var isValid = await Send(new IsValidLessonSubmissionPostRequest(request.LessonSubmissionId), token);
        if (!isValid) return null;

        // It would be much better to move the creation of the view into a separate service
        // Here is the thing. for the review itself a message must be inserted, but also for each response on an answer
        var messages = new List<Message>();
        var objectView = await CreateLessonSubmissionView(request.LessonSubmissionId, token);
        if (objectView == null) return null;

        var submission = await Send(new GetFinalSubmissionByIdRequest(request.LessonSubmissionId), token);
        var senderId = submission?.Enrollment?.MentorId;
        if (senderId == null) return null;

        var channel = await Send(new GetPersonalChannelRequest(senderId.Value, submission.Enrollment.StudentId), token);
        if (channel == null) return null;

        var reviewMsgRequest = new PostObjectChatMessageRequest
        {
            Message = objectView,
            Type = MessageTypes.MentorLessonSubmissionReview,
            ChannelId = channel.Id,
            ReferenceId = submission.Id,
            SenderId = senderId.Value
        };
        messages.Add(await Send(reviewMsgRequest, token));

        // Now loop through the answer and see which ones has a response
        var responses = submission.Answers.Where(c => string.IsNullOrEmpty(c.MentorReview) == false);
        foreach (var response in responses)
        {
            var view = CreateView(response, submission);
            if (view == null) continue;
            var answerMsgRequest = new PostObjectChatMessageRequest
            {
                ReferenceId = response.Id,
                Message = view,
                Type = MessageTypes.MentorAnswerReview,
                ChannelId = channel.Id,
                SenderId = senderId.Value,
            };
            messages.Add(await Send(answerMsgRequest, token));
        }

        return messages;
    }

    private static ReviewedAnswerView CreateView(QuestionAnswer response, LessonSubmission submission)
    {
        var question = submission?.Lesson?.Content?.FirstOrDefault(c => c.Id == response.QuestionId && c.Type == response.QuestionType);
        if (question == null) return null;

        return response.QuestionType switch
        {
            ContentBlockTypes.QuestionOpen => new ReviewedAnswerView(response, question as OpenQuestion),
            ContentBlockTypes.QuestionMultipleChoice => new ReviewedAnswerView(response, question as MultipleChoiceQuestion),
            ContentBlockTypes.QuestionBoolean => new ReviewedAnswerView(response, question as BoolQuestion),
            _ => null
        };
    }
}