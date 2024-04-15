using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services
    .AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/api-gateway-demo";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidAudiences = ["account"],
            ValidIssuers = ["http://localhost:8080/realms/api-gateway-demo"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddControllers();
builder.Services.AddAuthorization();

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
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .RequireAuthorization()
    .WithOpenApi();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}