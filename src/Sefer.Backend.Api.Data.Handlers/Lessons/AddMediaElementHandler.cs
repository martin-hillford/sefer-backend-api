namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class AddMediaElementHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddMediaElementRequest, MediaElement>(serviceProvider);