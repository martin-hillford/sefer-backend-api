namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetPreviousFeedbackHandler(IServiceProvider serviceProvider)
    : Handler<GetPreviousFeedbackRequest, List<QuestionAnswer>>(serviceProvider)
{
    public override async Task<List<QuestionAnswer>> Handle(GetPreviousFeedbackRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var subQuery = context.Answers.Where(a => a.Submission.Enrollment.CourseRevisionId == request.CourseRevisionId);
        return await context.Answers
            .Where(a =>
                subQuery.Any(s => s.QuestionId == a.QuestionId && s.QuestionType == a.QuestionType) &&
                a.Submission.Enrollment.MentorId == request.MentorId &&
                a.MentorReview != null &&
                a.MentorReview != ""
            )
            .ToListAsync(token);
    }
}

