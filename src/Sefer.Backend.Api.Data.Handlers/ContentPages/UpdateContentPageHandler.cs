namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class UpdateContentPageHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateContentPageRequest, ContentPage>(serviceProvider);