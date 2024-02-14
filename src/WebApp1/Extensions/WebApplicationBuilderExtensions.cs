using System.Net.Http.Headers;
using QuestPDF;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using Serilog;
using WebApp1.Data.FileManager;
using WebApp1.Data.Stores;
using WebApp1.External.Qtickets;
using WebApp1.External.SmsRu;
using WebApp1.Jobs;
using WebApp1.Services.ClientService;
using WebApp1.Services.EmailSender;
using WebApp1.Services.EventService;
using WebApp1.Services.PdfGenerator;
using WebApp1.Services.PhoneConfirmationService;
using WebApp1.Services.PhoneConfirmationService.PhoneCaller;
using WebApp1.Services.PrintService;
using WebApp1.Services.QrCodeGenerator;
using WebApp1.Services.SmsSender;
using WebApp1.Services.TemplateService;
using WebApp1.Services.TicketService;
using WebApp1.Services.TokenService;

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
        services.AddSingleton<IPhoneCaller, PhoneCaller>();

        services.AddScoped<IPhoneConfirmationService, PhoneConfirmationService>();

        return services;
    }

    public static WebApplicationBuilder SetupLogging(this WebApplicationBuilder builder)
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
        var basePath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot-user");

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
        Settings.License = LicenseType.Community;
        builder.Services.AddTransient<IPdfGenerator, PdfGenerator>();
        builder.Services.AddTransient<IQrCodeGenerator, QrCodeGenerator>();

        try
        {
            var fontPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot/fonts/Montserrat-Bold.ttf");
            using var stream = File.OpenRead(fontPath);
            FontManager.RegisterFont(stream);
        }
        catch
        {
            // ignored
        }

        return builder;
    }

    public static WebApplicationBuilder AddStores(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IScannerStore, ScannerStore>();
        builder.Services.AddScoped<IScreenStore, ScreenStore>();

        return builder;
    }

    public static WebApplicationBuilder AddCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IEventService, EventService>();
        builder.Services.AddScoped<ITicketService, TicketService>();
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<ITemplateService, TemplateService>();

        builder.Services.AddHttpClient("PrintService", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["PrintService:BaseApiUri"]!);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", builder.Configuration["PrintService:ApiKey"]);
        });
        builder.Services.AddScoped<IPrintService, PrintService>();

        return builder;
    }

    public static WebApplicationBuilder AddJobs(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<EventImportJob>();
        builder.Services.AddHostedService<ClientImportJob>();
        builder.Services.AddHostedService<TicketImportJob>();

        return builder;
    }
}