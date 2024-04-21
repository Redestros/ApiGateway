using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Transforms;

namespace Gateway.Config;

public static class ReverseProxyExtensions
{
    public static void AddReverseProxy(this IServiceCollection services, ConfigurationManager configuration)
    {
        var reverseProxyConfig = configuration.GetSection("ReverseProxy") ??
                                 throw new ArgumentException("ReverseProxy section is missing!");

        services.AddReverseProxy()
            .LoadFromConfig(reverseProxyConfig)
            .AddTransforms(builderContext =>
            {
                builderContext.AddRequestTransform(async transformContext =>
                {
                    var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");
                    if (accessToken != null)
                    {
                        transformContext.ProxyRequest.Headers.Remove("Cookie");
                        transformContext.ProxyRequest.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", accessToken);
                    }
                });
            });
    }
}