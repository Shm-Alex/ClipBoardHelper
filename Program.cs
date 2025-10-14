using Microsoft.AspNetCore.Mvc;
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

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClipBoardHelper v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run(); // ← Без аргументов!