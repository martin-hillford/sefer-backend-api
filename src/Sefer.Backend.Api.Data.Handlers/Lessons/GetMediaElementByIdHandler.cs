namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetMediaElementByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetMediaElementByIdRequest, MediaElement>(serviceProvider);