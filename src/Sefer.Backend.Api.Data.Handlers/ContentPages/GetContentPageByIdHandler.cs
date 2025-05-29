namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class GetContentPageByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetContentPageByIdRequest, ContentPage>(serviceProvider)
{ }