namespace Sefer.Backend.Api.Notifications;

/// <summary>
/// This method helps with easy deployment of all the services in this library
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Add the notifications services to the party.
    /// The following dependencies must be added as wel
    /// ICryptographyService, IRepositoryProvider, IOptions[MailServiceOptions], IServiceProvider, IMailServiceBase
    /// </summary>
    public static IHostApplicationBuilder AddNotificationServices(this IHostApplicationBuilder builder)
    {
        // Add the options
        builder.Services.Configure<FireBaseOptions>(builder.Configuration.GetSection("FireBase"));

        // Add depending services
        builder.Services.AddScoped<IViewRenderService, ViewRenderService>();

        // Add all internal services
        builder.Services.AddSingleton<IMailService, MailService>();
        builder.Services.AddSingleton<IEmailDigestService, EmailDigestService>();
        builder.Services.AddSingleton<IFireBase, FireBase>();
        builder.Services.AddSingleton<IFireBaseService, FireBaseService>();
        builder.Services.AddSingleton<IWebSocketProvider, WebSocketProvider>();

        // Add the notification service as a hosted service
        builder.Services.AddHostedService<BackgroundService>();

        // Also add the direct notification provider
        builder.Services.AddSingleton<INotificationService, NotificationService>();

        // Also add signalR
        builder.Services.AddSignalR();

        // We are done adding stuff
        return builder;
    }

    /// <summary>
    /// This will ensure the application builder will user the notification services to the mix
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="chatRoute"></param>
    public static IApplicationBuilder UseNotificationServices(this IApplicationBuilder builder, string chatRoute)
    {
        builder.UseEndpoints(endpoints => endpoints.MapHub<ChatHub>(chatRoute));
        return builder;
    }
}