namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class GetChannelByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetChannelByIdRequest, Channel>(serviceProvider);