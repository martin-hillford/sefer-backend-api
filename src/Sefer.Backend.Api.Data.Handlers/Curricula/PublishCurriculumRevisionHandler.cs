namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class PublishCurriculumRevisionHandler(IServiceProvider serviceProvider)
    : SyncHandler<PublishCurriculumRevisionRequest, bool>(serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override bool Handle(PublishCurriculumRevisionRequest request)
    {

        var context = GetDataContext();
        using var transaction = context.BeginTransaction();

        try
        {
            var helper = new PublishCurriculumRevisionHelper(_serviceProvider, context);
            var success = helper.Handle(request.CurriculumRevisionId);
            if (success) transaction.Commit();
            else transaction.Rollback();

            return success;
        }
        catch (Exception)
        {
            transaction.Rollback();

            return false;
        }
    }
}