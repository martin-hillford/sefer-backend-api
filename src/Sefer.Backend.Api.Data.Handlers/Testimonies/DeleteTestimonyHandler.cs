namespace Sefer.Backend.Api.Data.Handlers.Testimonies;

public class DeleteTestimonyHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteTestimonyRequest, Testimony>(serviceProvider);