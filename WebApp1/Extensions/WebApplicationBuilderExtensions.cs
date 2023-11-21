using QuestPDF.Infrastructure;
using Serilog;
using WebApp1.Data.FileManager;
using WebApp1.Data.Stores;
using WebApp1.External.Qtickets;
using WebApp1.External.SmsRu;
using WebApp1.Services.EmailSender;
using WebApp1.Services.PdfGenerator;
using WebApp1.Services.QrCodeGenerator;
using WebApp1.Services.SmsSender;

namespace WebApp1.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddQticketsApiProvider(this IServiceCollection services)
    {
        services.AddHttpClient("Qtickets", client => { client.BaseAddress = new Uri("https://qtickets.ru/api/rest/v1/"); });
        services.AddSingleton<IQticketsApiProvider, QticketsApiProvider>();

        return services;
    }

    public static IServiceCollection AddMessageSenders(this IServiceCollection services)
    {
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddTransient<ISmsSender, SmsSender>();

        services.AddHttpClient("SmsRu", client => { client.BaseAddress = new Uri("https://sms.ru/"); });
        services.AddSingleton<ISmsRuApiProvider, SmsRuApiProvider>();

        return services;
    }

    public static WebApplicationBuilder SetUpLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name ?? "Program")
                .Enrich.WithProperty("Environment", ctx.HostingEnvironment);
        });

        builder.Services.AddLogging();
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        return builder;
    }

    public static WebApplicationBuilder AddFileManagers(this WebApplicationBuilder builder)
    {
        var basePath = builder.Environment.WebRootPath;

        var logoFileManager = new FileManager(basePath, builder.Configuration["Path:LogoImagePath"]!);
        var backgroundFileManager = new FileManager(basePath, builder.Configuration["Path:BackgroundImagePath"]!);

        var logoImageManager = new ImageManager(logoFileManager);
        var backgroundImageManager = new ImageManager(backgroundFileManager);

        builder.Services.AddKeyedSingleton<IImageManager>("Logo", logoImageManager);
        builder.Services.AddKeyedSingleton<IImageManager>("Background", backgroundImageManager);

        return builder;
    }

    public static WebApplicationBuilder AddMediaGenerationServices(this WebApplicationBuilder builder)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        builder.Services.AddTransient<IPdfGenerator, PdfGenerator>();
        builder.Services.AddTransient<IQrCodeGenerator, QrCodeGenerator>();

        return builder;
    }

    public static WebApplicationBuilder AddStores(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IScannerStore, ScannerStore>();

        return builder;
    }
}