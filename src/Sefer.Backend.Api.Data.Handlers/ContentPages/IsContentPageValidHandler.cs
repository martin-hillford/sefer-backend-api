namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class IsContentPageValidHandler(IServiceProvider serviceProvider)
    : IsValidEntityHandler<IsContentPageValidRequest, ContentPage>(serviceProvider);