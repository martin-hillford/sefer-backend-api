namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class DeleteLessonChoiceHandler(IServiceProvider serviceProvider)
    : Handler<DeleteLessonChoiceRequest, bool>(serviceProvider)
{
    public override Task<bool> Handle(DeleteLessonChoiceRequest request, CancellationToken _)
    {
        var result = Handle(request);
        return Task.FromResult(result);
    }

    private bool Handle(DeleteLessonChoiceRequest request)
    {
        var context = GetDataContext();
        var transaction = context.BeginTransaction();

        try
        {
            foreach (var choice in request.Choices)
            {
                var dbChoice = context.LessonMultipleChoiceQuestionChoices.SingleOrDefault(c => c.Id == choice.Id);
                if(dbChoice != null) context.Remove(dbChoice);
            }

            context.SaveChanges();
            transaction.Commit();

            return true;
        }
        catch (Exception)
        {
            transaction.Rollback();
            return false;
        }
    }
}