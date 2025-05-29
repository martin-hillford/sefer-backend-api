namespace Sefer.Backend.Api.Data.Handlers.Testimonies;

public class AddTestimonyHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddTestimonyRequest, Testimony>(serviceProvider);