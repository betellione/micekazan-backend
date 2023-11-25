using Micekazan.PrintDispatcher.Auth.ApiKey;
using Microsoft.Extensions.Options;

namespace Micekazan.PrintDispatcher.Auth;

public class ApiKeyEndpointFilter : IEndpointFilter
{
    private readonly ApiKeyOptions _options;

    public ApiKeyEndpointFilter(IOptions<ApiKeyOptions> optionsAccessor)
    {
        _options = optionsAccessor.Value;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var header = context.HttpContext.Request.Headers.Authorization;
        if (header.Count != 1 || header[0] != _options.ApiKey) return Results.Unauthorized();
        return await next(context);
    }
}