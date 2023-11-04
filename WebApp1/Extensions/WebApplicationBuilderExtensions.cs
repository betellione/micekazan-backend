using WebApp1.External.Qtickets;

namespace WebApp1.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddQticketsApiProvider(this IServiceCollection services)
    {
        services.AddHttpClient("Qtickets", client => { client.BaseAddress = new Uri("https://qtickets.ru/api/rest/v1/"); });
        services.AddSingleton<IQticketsApiProvider, QticketsApiProvider>();

        return services;
    }

    public static IServiceCollection AddTicketService(this IServiceCollection builder)
    {
        throw new NotImplementedException();
    }
}