using Micekazan.PrintDispatcher.Data.FileManager;

namespace Micekazan.PrintDispatcher.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddFileManagers(this WebApplicationBuilder builder)
    {
        var basePath = builder.Environment.WebRootPath;

        var fileManager = new FileManager(basePath, builder.Configuration["Path:PdfPath"]!);
        var pdfManager = new PdfManager(fileManager);

        builder.Services.AddSingleton<IPdfManager>(pdfManager);

        return builder;
    }
}