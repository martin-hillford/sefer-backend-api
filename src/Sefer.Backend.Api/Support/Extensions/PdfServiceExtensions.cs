using Microsoft.Extensions.Hosting;
using Sefer.Backend.Api.Services.Pdf;

namespace Sefer.Backend.Api.Support.Extensions;

public static class PdfServiceExtensions
{
    public static IHostApplicationBuilder AddPdfService(this IHostApplicationBuilder builder, string section = "Pdf")
    {
        builder.Services.Configure<PdfOptions>(builder.Configuration.GetSection(section));
        builder.Services.AddSingleton<IPdfRenderService, PdfRenderService>();
        return builder;
    }
}