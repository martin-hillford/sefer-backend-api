namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class UpdateMediaElementHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateMediaElementRequest, MediaElement>(serviceProvider);