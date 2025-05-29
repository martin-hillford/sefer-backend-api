namespace Sefer.Backend.Api.Data.Handlers.Blogs;

public class IsBlogValidHandler(IServiceProvider serviceProvider)
    : IsValidEntityHandler<IsBlogValidRequest, Blog>(serviceProvider);