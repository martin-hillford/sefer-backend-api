namespace Sefer.Backend.Api.Data.Handlers.Testimonies;

public class UpdateTestimonyHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateTestimonyRequest, Testimony>(serviceProvider);