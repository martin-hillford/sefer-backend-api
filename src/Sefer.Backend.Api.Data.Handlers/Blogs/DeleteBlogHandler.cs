// This handler is initiated through reflection
// ReSharper disable ClassNeverInstantiated.Global
namespace Sefer.Backend.Api.Data.Handlers.Blogs;

public class DeleteBlogHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteBlogRequest, Blog>(serviceProvider);