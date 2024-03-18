using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Gateway.Transformers;

public class CookieTransformer : ITransformProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieTransformer(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void ValidateRoute(TransformRouteValidationContext context)
    {
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
    }

    public void Apply(TransformBuilderContext context)
    {
        context.AddRequestTransform(async transformContext =>
        {
            if (_httpContextAccessor.HttpContext == null) return;

            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(accessToken) == false)
            {
                transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                transformContext.ProxyRequest.Headers.Remove("Cookie");
            }
        });
    }
}