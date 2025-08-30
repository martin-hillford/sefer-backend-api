using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Sefer.Backend.Api.Data.Algorithms;
using Sefer.Backend.Api.Notifications;
using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Services.Logging;
using Sefer.Backend.Api.Services.Mail;
using Sefer.Backend.GeoIP.Lib;
using Sefer.Backend.Support.Lib.Cors;

namespace Sefer.Backend.Api;

/// <summary>
/// The main program that starts the application
/// </summary>
public static class Program
{
    /// <summary>
    /// The main method, start the web application
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).Configure();
        var application = builder.Build().SetupMiddleware();
        application.Run();
    }

    private static WebApplicationBuilder Configure(this WebApplicationBuilder builder)
    {
        builder     
            .WithSharedConfig()
            .AddCleverReach("CleverReach")
            .AddShortUrlService()
            .AddGeoIPService("GeoIP")
            .AddAvatarService()
            .AddCustomCorsMiddleware()
            .AddPdfService()
            .AddDatabase()
            .AddMailService()
            .AddDataProtection()
            .AddFileStorage()
            .AddNotificationServices()
            .AddAudioStorageService()
            .AddConfiguration<SecurityOptions>("Security")
            .AddConfiguration<PaymentOptions>("Payment")
            
            .AddSwagger();

        builder.Services
            .Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 4294967296; })
            .AddTokenAuthentication()
            .AddSingleton<IHttpClient, HttpClientWrapper>()
            .AddSingleton<ICryptographyService, CryptographyService>()
            .AddSingleton<IPasswordService, PasswordService>()
            .AddScoped<IUserAuthenticationService, UserAuthenticationService>()
            .AddSingleton<IMentorAssigningFactory, MentorAssigningFactory>()
            .AddDatabaseLogging()
            .AddResponseCaching()
            .AddMemoryCache()
            .AddHttpContextAccessor()
            .AddHttpClient()
            .AddControllersAndPages();

        return builder;
    }

    private static WebApplication SetupMiddleware(this WebApplication application)
    {
        // Allow the cors header for specific places
        application.UseCustomCorsMiddleware();

        // Deal with logging for api requests
        application.UseRequestLogger();

        if (EnvVar.IsDevelopmentEnv())
        {
            application.UseDeveloperExceptionPage();
            application.UseSwaggerWithToken();
        }
        else
        {
            application.UseExceptionHandler("/Error");
            application.UseHsts();
        }

        // And use cors, the documentation and mvc
        application.UseRouting();
        application.UseResponseCaching();
        application.UseAuthorization();
        application.UseNotificationServices("/chat/server");

#pragma warning disable ASP0014
        application.UseEndpoints(endpoints => endpoints.MapControllers());
        return application;
#pragma warning restore ASP0014        
    }
}