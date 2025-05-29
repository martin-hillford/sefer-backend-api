using Microsoft.Extensions.Hosting;

namespace Sefer.Backend.Api.Support.Extensions;

public static class PdfServiceExtensions
{
    public static IHostApplicationBuilder AddPdfService(this IHostApplicationBuilder builder, string section = "Pdf")
    {
        builder.Services.Configure<PdfOptions>(builder.Configuration.GetSection(section));
        return builder;
    }
}