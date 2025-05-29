namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class DeleteMediaElementHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteMediaElementRequest, MediaElement>(serviceProvider);