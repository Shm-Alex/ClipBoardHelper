var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<TextRequestBodyFilter>();
    c.SwaggerDoc("v1", new() { Title = "ClipBoardHelper API", Version = "v1" });
});

// 🔜 Заглушка для будущей авторизации
// builder.Services.AddAuthentication(...);
// builder.Services.AddAuthorization();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClipBoardHelper v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization(); // ← оставляем, даже без аутентификации — не мешает
app.MapControllers();


app.Run($"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}");