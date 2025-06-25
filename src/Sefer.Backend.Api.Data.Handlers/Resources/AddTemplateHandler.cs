namespace Sefer.Backend.Api.Data.Handlers.Resources;

public class AddTemplateHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddTemplateRequest, Template>(serviceProvider);