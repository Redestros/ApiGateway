using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Config;

public static class OauthProxyExtension
{
    public static void AddOAuthProxy(this IServiceCollection services)
    {
        services.AddSingleton<CookieOidcRefresher>();
        
        var proxyOptions = services.GetOptions<OAuthProxyOptions>(OAuthProxyOptions.SectionName);
        
        services.AddAuthentication(options =>
            {
                //Sets cookie authentication scheme
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(cookie =>
            {
                //Sets the cookie name and max-age, so the cookie is invalidated.
                cookie.Cookie.Name = "keycloak.cookie";
                cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                // cookie.SlidingExpiration = true;
                cookie.SessionStore = new RedisSessionStore(services);
            })
            .AddOpenIdConnect(options =>
            {
                //Use default signin scheme
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.Authority = proxyOptions.Authority;
                options.RequireHttpsMetadata = false;

                options.ClientId = proxyOptions.ClientId;
                options.ClientSecret = proxyOptions.ClientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                //SameSite is needed for Chrome/Firefox, as they will give http error 500 back, if not set to unspecified.
                options.NonceCookie.SameSite = SameSiteMode.Unspecified;
                options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;

                // options.Scope.Clear();
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");

                // options.MapInboundClaims = false; // Don't rename claim types
                // options.CallbackPath = "/signin-oidc"; // Set the callback path
                // options.SignedOutCallbackPath = ""; // Set the sign-out callback path
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateTokenReplay = true,
                    NameClaimType = "preferred_username",
                    RoleClaimType = "roles"
                };
                options.CallbackPath= "/signin-oidc";
                options.SaveTokens = true;
            });
        
        services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
            .Configure<CookieOidcRefresher>((cookieOptions, refresher) =>
            {
                cookieOptions.Events.OnValidatePrincipal = context =>
                    refresher.ValidateOrRefreshCookieAsync(context, OpenIdConnectDefaults.AuthenticationScheme);
            });
    }

    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("authenticatedUser", policy => { policy.RequireAuthenticatedUser(); });
    }
}