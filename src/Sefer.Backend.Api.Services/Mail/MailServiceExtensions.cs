using Microsoft.Extensions.Hosting;

namespace Sefer.Backend.Api.Services.Mail;

public static class MailServiceExtensions
{
    public static IHostApplicationBuilder AddMailService(this IHostApplicationBuilder builder, string section = "Mail")
    {
        builder.Services.AddSingleton<ISmtpClientProvider, SmtpClientProvider>();
        builder.Services.Configure<MailServiceOptions>(builder.Configuration.GetSection(section));
        builder.Services.AddSingleton<IMailServiceBase, MailServiceBase>();
        return builder;
    }
}
