using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Sefer.Backend.Api.Data;
using Sefer.Backend.Api.Data.Handlers;
using Sefer.Backend.Api.Services.FileStorage.AzureStorage;

namespace Sefer.Backend.Api.Support.Extensions;

public static class StartupExtensions
{
    public static IHostApplicationBuilder AddDatabase(this IHostApplicationBuilder builder)
    {
        // Get the configuration of the database
        var connectionString = builder.Configuration["Database:ConnectionString"];
        var databaseType = builder.Configuration["Database:Type"];

        switch (databaseType)
        {
            case "SQLServer":
                builder.Services.AddDbContextPool<DataContext>(context => context.UseSqlServer(connectionString));
                break;
            case "InMemory":
                var databaseName = "InMemoryTestDatabase-" + Guid.NewGuid();
                builder.Services.AddDbContextPool<DataContext>(options => options.UseInMemoryDatabase(databaseName));
                break;
            default:
                builder.Services.AddPostgres(connectionString);
                break;
        }
        
        // add the common services
        builder.Services
            .AddMediation()
            .AddSingleton<IDataContextProvider, DataContextProvider>();

        return builder;
    }

    private static void AddPostgres(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DataContext>(context =>
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            context
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        }, ServiceLifetime.Singleton);
    }

    public static IHostApplicationBuilder AddFileStorage(this IHostApplicationBuilder builder)
    {
        // Load the configuration to use from the configuration file
        var section = builder.Configuration.GetSection("Storage");
        var method = section["Method"];

        // Load the appropriated storage provider
        switch (method)
        {
            case "FileSystem":
                builder.Services.Configure<FileSystemStorageServiceOptions>(section.GetSection("Paths"));
                builder.Services.AddSingleton<IFileStorageService, FileSystemStorageService>();
                break;
            case "Azure":
                builder.Services.Configure<AzureStorageServiceOptions>(section.GetSection("Blob"));
                builder.Services.AddScoped<IFileStorageService, AzureStorageService>();
                break;
            default:
                throw new Exception("No FileStorage configured.");
        }

        return builder;
    }

    public static IHostApplicationBuilder AddDataProtection(this IHostApplicationBuilder builder)
    {
        var directory = builder.Configuration["DataProtection:PersistKeysToFileSystem"];
        if (string.IsNullOrEmpty(directory)) builder.Services.AddDataProtection();
        else builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(directory));
        return builder;
    }

    public static IHostApplicationBuilder AddSwagger(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSwaggerWithToken();
        builder.Services.AddSwaggerGen(o =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        builder.Services.ConfigureSwaggerGen(c => { c.CustomSchemaIds(x => x.FullName); });
        return builder;
    }

    public static IServiceCollection AddControllersAndPages(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(DefaultJsonOptions.SetOptions);
        return services;
    }

    public static IHostApplicationBuilder AddConfiguration<T>(this IHostApplicationBuilder builder, string section) where T : class
    {
        builder.Services.Configure<T>(builder.Configuration.GetSection(section));
        return builder;
    }
}