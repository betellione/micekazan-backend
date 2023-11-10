using Serilog;

namespace WebApp1.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}