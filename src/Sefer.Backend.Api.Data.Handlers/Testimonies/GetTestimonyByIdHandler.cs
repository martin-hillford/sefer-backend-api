namespace Sefer.Backend.Api.Data.Handlers.Testimonies;

public class GetTestimonyByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetTestimonyByIdRequest, Testimony>(serviceProvider);