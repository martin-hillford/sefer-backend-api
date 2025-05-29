namespace Sefer.Backend.Api.Data.Handlers.Testimonies;

public class GetTestimoniesHandler(IServiceProvider serviceProvider)
    : GetEntitiesHandler<GetTestimoniesRequest, Testimony>(serviceProvider);