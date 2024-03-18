using Gateway;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddReverseProxy(builder.Configuration);

builder.Services.AddAuthentication(options =>
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
        cookie.Cookie.MaxAge = TimeSpan.FromMinutes(60);
        cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        cookie.SlidingExpiration = true;
        cookie.SessionStore = new RedisSessionStore(builder.Services);
    })
    .AddOpenIdConnect(options =>
    {
        //Use default signin scheme
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        
        options.Authority = "http://localhost:8080/realms/user-multi-profile-demo";
        options.RequireHttpsMetadata = false;

        options.ClientId = "student-app";
        options.ClientSecret = "Xwl4G0Y1d9Ypt7VBYyVSzPUSmK1CYj1G";
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
        options.SignedOutCallbackPath = "/test"; // Set the sign-out callback path
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            NameClaimType = "preferred_username",
            RoleClaimType = "roles"
        };

        options.SaveTokens = true;
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("custom", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AuthenticationSchemes = [OpenIdConnectDefaults.AuthenticationScheme];
    });


var connection = builder.Configuration.GetConnectionString("Redis")!;

builder.Services.AddStackExchangeRedisCache(option =>
{
    option.Configuration = connection;
});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapReverseProxy();

app.Run();

