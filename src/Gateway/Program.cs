using Gateway.Config;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddReverseProxy(builder.Configuration);

builder.Services.AddOAuthProxy();

builder.Services.AddAuthorizationPolicies();

builder.Services.AddCache(builder.Configuration);

const string corsPolicy = "defaultCorsPolicy";
builder.Services.AddCors(options => options.AddPolicy(corsPolicy,
    configurePolicy => configurePolicy
        .WithOrigins("http://localhost:4200", "https://localhost:7285", "http://localhost:8080")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()));

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

app.UseCors(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapReverseProxy();

app.Run();

