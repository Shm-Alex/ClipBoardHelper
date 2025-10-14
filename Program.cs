using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;

// 👇 Добавьте это ДО создания builder
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PORT")))
{
    // Render, Fly.io и другие PaaS передают PORT
    // ASP.NET Core читает ASPNETCORE_URLS
    Environment.SetEnvironmentVariable("ASPNETCORE_URLS", $"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT")}");
}

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<TextRequestBodyFilter>();
    c.SwaggerDoc("v1", new() { Title = "ClipBoardHelper API", Version = "v1" });
});

var app = builder.Build();
// 🔍 Middleware: логировать каждый входящий запрос
app.Use(async (context, next) =>
{
var logger = app.Services.GetRequiredService<ILogger<Program>>();
    try
    {

        logger.LogInformation(">>> REQUEST: {Method} {Url} | Headers: {Headers}",
        context.Request.Method,
        context.Request.GetDisplayUrl(),
        string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}={h.Value}"))
        );
    }
    catch (Exception e)
    {
        
        logger.LogWarning(e,"Что то залогировать не смогли ");
    }
await next();

logger.LogInformation("<<< RESPONSE: {StatusCode}", context.Response.StatusCode);
});

// Configure pipeline
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClipBoardHelper v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run(); // ← Без аргументов!