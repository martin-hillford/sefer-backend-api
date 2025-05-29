namespace Sefer.Backend.Api.Data.Handlers.Blogs;

// This handler is initiated through reflection
// ReSharper disable ClassNeverInstantiated.Global
public class UpdateBlogHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateBlogRequest, Blog>(serviceProvider);