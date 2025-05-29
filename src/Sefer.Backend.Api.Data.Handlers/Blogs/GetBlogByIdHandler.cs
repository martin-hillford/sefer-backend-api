// This handler is initiated through reflection
// ReSharper disable ClassNeverInstantiated.Global
namespace Sefer.Backend.Api.Data.Handlers.Blogs;

public class GetBlogByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetBlogByIdRequest, Blog>(serviceProvider);