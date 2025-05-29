namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class AddContentPageHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddContentPageRequest, ContentPage>(serviceProvider);