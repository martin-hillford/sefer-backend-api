namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class IsCurriculumRevisionPublishableHandler(IServiceProvider serviceProvider)
    : SyncHandler<IsCurriculumRevisionPublishableRequest, bool>(serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override bool Handle(IsCurriculumRevisionPublishableRequest request)
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.SingleOrDefault(r => r.Id == request.CurriculumRevisionId);

        var helper = new IsCurriculumRevisionPublishableHelper(_serviceProvider, context);
        var isPublishable = helper.IsPublishable(revision);
        context.Dispose();

        return isPublishable;
    }
}